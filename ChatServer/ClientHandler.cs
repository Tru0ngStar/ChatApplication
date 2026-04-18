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
        private int _currentGroupId = -1; // Changed from string to int (-1 means not in a group)

        public ClientHandler(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public async Task HandleClientAsync()
        {
            try
            {
                byte[] buffer = new byte[10 * 1024 * 1024]; // 10MB buffer for files
                while (IsConnected)
                {
                    int read = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0) break;
                    string json = Encoding.UTF8.GetString(buffer, 0, read);

                    var packets = json.Replace("}{", "}|{").Split('|');
                    foreach (var p in packets)
                    {
                        if (string.IsNullOrWhiteSpace(p)) continue;
                        var packet = JsonSerializer.Deserialize<Packet>(p);
                        if (packet != null) await ProcessPacketAsync(packet);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {Username}: {ex.Message}"); }
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
                    if (logOk) await Program.UpdateAllClientsUserListAsync();
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
                        Console.WriteLine($"[FILE] {Username} gửi file: {packet.FileName} ({packet.FileSize} bytes)");

                         // Lưu file vào disk
                        try
                        {
                            string uploadsDir = Path.Combine(AppContext.BaseDirectory, "Uploads");
                            Directory.CreateDirectory(uploadsDir);

                            string filePath = Path.Combine(uploadsDir, packet.FileName ?? "unnamed_file");

                            // Tránh ghi đè file
                            int counter = 1;
                            string baseFileName = Path.GetFileNameWithoutExtension(filePath);
                            string extension = Path.GetExtension(filePath);
                            while (File.Exists(filePath))
                            {
                                filePath = Path.Combine(uploadsDir, $"{baseFileName}_{counter}{extension}");
                                counter++;
                            }

                            if (packet.FileData != null && packet.FileData.Length > 0)
                            {
                                File.WriteAllBytes(filePath, packet.FileData);
                                Console.WriteLine($"[FILE] Đã lưu file: {filePath}");

                                // Lưu thông tin file vào database
                                DatabaseContext.SaveFileUpload(
                                    packet.FileName ?? "unnamed_file",
                                    packet.FileSize,
                                    Username ?? "Unknown",
                                    filePath,
                                    _currentGroupId
                                );
                            }

                            // Broadcast File packet cho tất cả clients (để hiển thị download button)
                            var filePacket = new Packet(PacketType.File)
                            {
                                Sender = Username,
                                FileName = packet.FileName,
                                FileSize = packet.FileSize,
                                // FileData = null (không gửi dữ liệu file lại - chỉ metadata)
                                GroupId = _currentGroupId
                            };
                            await Program.BroadcastToGroupAsync(_currentGroupId, filePacket, this);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Lỗi lưu file: {ex.Message}");
                        }
                    }
                    break;

                case PacketType.CreateGroup:
                    if (!string.IsNullOrEmpty(Username) && packet.GroupId >= 0)
                    {
                        Program.AddClientToGroup(packet.GroupId, this);
                        _currentGroupId = packet.GroupId;
                        var msg = new Packet(PacketType.Message) 
                        { 
                            Sender = "HỆ THỐNG", 
                            Content = $"{Username} đã tạo nhóm: Group {packet.GroupId + 1}",
                            GroupId = packet.GroupId
                        };
                        await Program.BroadcastToGroupAsync(packet.GroupId, msg);
                    }
                    break;

                case PacketType.JoinGroup:
                    if (!string.IsNullOrEmpty(Username) && packet.GroupId >= 0)
                    {
                        Program.AddClientToGroup(packet.GroupId, this);
                        _currentGroupId = packet.GroupId;
                        var msg = new Packet(PacketType.Message) 
                        { 
                            Sender = "HỆ THỐNG", 
                            Content = $"{Username} đã tham gia nhóm: Group {packet.GroupId + 1}",
                            GroupId = packet.GroupId
                        };
                        await Program.BroadcastToGroupAsync(packet.GroupId, msg);
                    }
                    break;

                case PacketType.GroupMessage:
                    if (!string.IsNullOrEmpty(Username) && _currentGroupId >= 0)
                    {
                        Console.WriteLine($"[GROUP {_currentGroupId}] {Username}: {packet.Content}");
                        packet.Type = PacketType.Message; // Convert to regular message for display
                        packet.GroupId = _currentGroupId; // Ensure packet has correct GroupId

                        // Không lưu tin nhắn vào database - chỉ hiển thị trực tiếp
                        // DatabaseContext.SaveChatMessage(Username, packet.Content, _currentGroupId);

                        // Lưu vào file history
                        ChatHistoryManager.SaveMessage(_currentGroupId, Username, packet.Content, "Message");

                        await Program.BroadcastToGroupAsync(_currentGroupId, packet, this);
                    }
                    break;

                case PacketType.LeaveGroup:
                    if (!string.IsNullOrEmpty(Username) && packet.GroupId >= 0)
                    {
                        Program.RemoveClientFromGroup(packet.GroupId, this);
                        if (_currentGroupId == packet.GroupId)
                        {
                            _currentGroupId = -1;
                        }
                        var msg = new Packet(PacketType.Message)
                        {
                            Sender = "HỆ THỐNG",
                            Content = $"{Username} đã rời khỏi nhóm: Group {packet.GroupId + 1}",
                            GroupId = packet.GroupId
                        };
                        await Program.BroadcastToGroupAsync(packet.GroupId, msg);
                    }
                    break;

                case PacketType.FileDownloadRequest:
                    if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(packet.FileName))
                    {
                        try
                        {
                            string uploadsDir = Path.Combine(AppContext.BaseDirectory, "Uploads");
                            string filePath = Path.Combine(uploadsDir, packet.FileName);

                            if (File.Exists(filePath))
                            {
                                byte[] fileData = File.ReadAllBytes(filePath);
                                var downloadPacket = new Packet(PacketType.FileDownloadResponse)
                                {
                                    FileName = packet.FileName,
                                    FileSize = fileData.Length,
                                    FileData = fileData
                                };
                                await SendPacketAsync(downloadPacket);
                                Console.WriteLine($"[FILE] Đã gửi file download: {packet.FileName}");
                            }
                            else
                            {
                                Console.WriteLine($"[ERROR] File không tồn tại: {filePath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Lỗi download file: {ex.Message}");
                        }
                    }
                    break;
            }
        }

        public async Task SendAsync(byte[] data) => await _stream.WriteAsync(data, 0, data.Length);
        private async Task SendPacketAsync(Packet p) => await SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(p)));

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public void Disconnect()
        {
            if (_currentGroupId >= 0)
            {
                Program.RemoveClientFromGroup(_currentGroupId, this);
            }
            _client.Close();
            Program.RemoveClient(this);
            if (!string.IsNullOrEmpty(Username))
            {
                _ = Program.UpdateAllClientsUserListAsync();
            }
        }
    }
}