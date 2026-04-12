using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ChatShared;

namespace ChatServer
{
    public partial class Program
    {
        private static TcpListener? _listener;
        private static readonly List<ClientHandler> _clients = new();
        private static readonly Dictionary<string, List<ClientHandler>> _groups = new();
        private static readonly object _clientLock = new();
        private const int Port = 9999;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            DatabaseContext.Initialize();

            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();

            Console.WriteLine("[DB] Cơ sở dữ liệu tài khoản đã sẵn sàng.");
            Console.WriteLine("======================================");
            Console.WriteLine($" SERVER CHAT ĐÃ KHỞI ĐỘNG");
            Console.WriteLine($" Cổng: {Port}");
            Console.WriteLine($" Thời gian: {DateTime.Now}");
            Console.WriteLine("======================================");

            try
            {
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"[LOG] Client mới kết nối từ: {client.Client.RemoteEndPoint}");
                    var handler = new ClientHandler(client);
                    lock (_clientLock)
                    {
                        _clients.Add(handler);
                    }
                    _ = Task.Run(() => handler.HandleClientAsync());
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {ex.Message}"); }
        }

        public static async Task BroadcastAsync(Packet packet, ClientHandler? exclude = null)
        {
            var json = JsonSerializer.Serialize(packet);
            var data = Encoding.UTF8.GetBytes(json);
            List<ClientHandler> clientsCopy;
            lock (_clientLock)
            {
                clientsCopy = _clients.ToList();
            }
            foreach (var client in clientsCopy)
            {
                if (client == exclude || !client.IsConnected) continue;
                try { await client.SendAsync(data); } catch { client.Disconnect(); }
            }
        }

        public static async Task BroadcastToGroupAsync(string groupName, Packet packet, ClientHandler? exclude = null)
        {
            if (!_groups.ContainsKey(groupName)) return;
            var json = JsonSerializer.Serialize(packet);
            var data = Encoding.UTF8.GetBytes(json);
            foreach (var client in _groups[groupName].ToList())
            {
                if (client == exclude || !client.IsConnected) continue;
                try { await client.SendAsync(data); } catch { client.Disconnect(); }
            }
        }

        public static void AddClientToGroup(string groupName, ClientHandler handler)
        {
            if (!_groups.ContainsKey(groupName)) _groups[groupName] = new();
            if (!_groups[groupName].Contains(handler)) _groups[groupName].Add(handler);
            Console.WriteLine($"[GROUP] {handler.Username} tham gia nhóm: {groupName}");
        }

        public static void RemoveClientFromGroup(string groupName, ClientHandler handler)
        {
            if (_groups.ContainsKey(groupName)) _groups[groupName].Remove(handler);
        }

        public static List<string> GetOnlineUsers()
        {
            lock (_clientLock)
            {
                return _clients.Where(c => !string.IsNullOrEmpty(c.Username)).Select(c => c.Username).ToList();
            }
        }

        public static async Task UpdateAllClientsUserListAsync()
        {
            var userList = GetOnlineUsers();
            var packet = new Packet(PacketType.UpdateUserList) { UserList = userList };
            await BroadcastAsync(packet);
        }

        public static void RemoveClient(ClientHandler handler)
        {
            lock (_clientLock)
            {
                _clients.Remove(handler);
            }
            Console.WriteLine($"[LOG] Một client đã thoát. Còn lại: {_clients.Count}");
        }
    }
}