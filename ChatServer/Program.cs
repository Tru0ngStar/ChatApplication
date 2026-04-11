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
        private const int Port = 9999;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            DatabaseContext.Initialize();

            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();

            Console.WriteLine("[DB] Cơ sở dữ liệu SQLite đã sẵn sàng.");
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
                    _clients.Add(handler);
                    _ = Task.Run(() => handler.HandleClientAsync());
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {ex.Message}"); }
        }

        public static async Task BroadcastAsync(Packet packet, ClientHandler? exclude = null)
        {
            var json = JsonSerializer.Serialize(packet);
            var data = Encoding.UTF8.GetBytes(json);
            foreach (var client in _clients.ToList())
            {
                if (client == exclude || !client.IsConnected) continue;
                try { await client.SendAsync(data); } catch { client.Disconnect(); }
            }
        }

        public static void RemoveClient(ClientHandler handler)
        {
            _clients.Remove(handler);
            Console.WriteLine($"[LOG] Một client đã thoát. Còn lại: {_clients.Count}");
        }
    }
}