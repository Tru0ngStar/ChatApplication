using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ChatShared;

namespace ChatServer
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        public string? Username { get; private set; }
        public bool IsConnected => _client.Connected;

        public ClientHandler(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public async Task HandleClientAsync()
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024];
                while (IsConnected)
                {
                    int read = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0) break;
                    string json = Encoding.UTF8.GetString(buffer, 0, read);

                    var packets = json.Replace("}{", "}|{").Split('|');
                    foreach (var p in packets)
                    {
                        var packet = JsonSerializer.Deserialize<Packet>(p);
                        if (packet != null) await ProcessPacketAsync(packet);
                    }
                }
            }
            catch { }
            finally { Disconnect(); }
        }

        private async Task ProcessPacketAsync(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.Register:
                    bool regOk = DatabaseContext.RegisterUser(packet.Sender!, packet.Password!);
                    Console.WriteLine($"[REG] Thử đăng ký: {packet.Sender} -> {(regOk ? "Thành công" : "Thất bại")}");
                    await SendPacketAsync(new Packet(PacketType.Register) { IsSuccess = regOk, Content = regOk ? "Thành công!" : "Tài khoản tồn tại!" });
                    break;

                case PacketType.Login:
                    bool logOk = DatabaseContext.ValidateUser(packet.Sender!, packet.Password!);
                    if (logOk)
                    {
                        Username = packet.Sender;
                        Console.WriteLine($"[AUTH] {Username} đã đăng nhập thành công.");
                    }
                    await SendPacketAsync(new Packet(PacketType.Login) { IsSuccess = logOk });
                    break;

                case PacketType.Message:
                    if (!string.IsNullOrEmpty(Username))
                    {
                        Console.WriteLine($"[MSG] {Username}: {packet.Content}");
                        await Program.BroadcastAsync(packet, this);
                    }
                    break;

                case PacketType.File:
                    if (!string.IsNullOrEmpty(Username))
                    {
                        Console.WriteLine($"[FILE] {Username} đang gửi file: {packet.FileName}");
                        await Program.BroadcastAsync(packet, this);
                    }
                    break;
            }
        }

        public async Task SendAsync(byte[] data) => await _stream.WriteAsync(data, 0, data.Length);
        private async Task SendPacketAsync(Packet p) => await SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(p)));
        public void Disconnect() { _client.Close(); Program.RemoveClient(this); }
    }
}