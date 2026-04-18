using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ChatShared;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private string? _currentUsername;
        private int _currentGroupId = -1; // Changed from string to int (-1 means not in a group)
        private List<ChatShared.FileInfo>? _currentFileList;
        private readonly Dictionary<int, string> _groupNames = new() // Map GroupId to display name
        {
            { 0, "Group 1" },
            { 1, "Group 2" },
            { 2, "Group 3" }
        };

        public Form1()
        {
            InitializeComponent();
            txtInput.KeyDown += TxtInput_KeyDown;
        }

        private async void btnLogin_Click(object sender, EventArgs e) => await Auth(PacketType.Login);
        private async void btnRegister_Click(object sender, EventArgs e) => await Auth(PacketType.Register);

        private async Task Auth(PacketType type)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                return;
            }

            if (!await Connect()) return;
            var p = new Packet(type) { Sender = txtUsername.Text, Password = txtPassword.Text };
            await SendPacket(p);
        }

        private async Task<bool> Connect()
        {
            if (_client?.Connected == true) return true;
            try
            {
                _client = new TcpClient("14.225.205.125", 9999);
                _stream = _client.GetStream();
                _ = Task.Run(Receive);
                return true;
            }
            catch { MessageBox.Show("Server chưa bật!"); return false; }
        }

        private async Task Receive()
        {
            byte[] buffer = new byte[10 * 1024 * 1024]; // 10MB buffer
            while (_client?.Connected == true)
            {
                int n = await _stream!.ReadAsync(buffer, 0, buffer.Length);
                if (n == 0) break;
                string json = Encoding.UTF8.GetString(buffer, 0, n);
                var parts = json.Replace("}{", "}|{").Split('|');

                foreach (var part in parts)
                {
                    if (string.IsNullOrWhiteSpace(part)) continue;
                    var p = JsonSerializer.Deserialize<Packet>(part);
                    if (p == null) continue;
                    this.Invoke(() => HandlePacket(p));
                }
            }
        }

        private void HandlePacket(Packet p)
        {
            switch (p.Type)
            {
                case PacketType.Login:
                    if (p.IsSuccess)
                    {
                        _currentUsername = txtUsername.Text;
                        grpChat.Enabled = true;
                        grpGroup.Enabled = true;
                        grpAuth.Enabled = false;
                        AppendSystemMessage("--- Đăng nhập thành công! ---", Color.Green);
                        RefreshFileList();
                    }
                    else
                    {
                        MessageBox.Show("Đăng nhập thất bại!");
                    }
                    break;

                case PacketType.Register:
                    MessageBox.Show(p.Content ?? "Kết quả đăng ký");
                    break;

                case PacketType.Message:
                    Color senderColor = p.Sender == _currentUsername ? Color.Blue : Color.Red;
                    if (p.Sender == "HỆ THỐNG") senderColor = Color.Green;
                    AppendMessageWithColor($"{p.Sender}: {p.Content}\n", senderColor);
                    break;

                case PacketType.File:
                    AppendSystemMessage($"📁 {p.Sender} gửi file: {p.FileName} ({FormatFileSize(p.FileSize)})", Color.Purple);
                    if (p.FileData != null && p.FileData.Length > 0)
                    {
                        SaveReceivedFile(p.FileName, p.FileData);
                    }
                    RefreshFileList();
                    break;

                case PacketType.UpdateUserList:
                    lstOnlineUsers.Items.Clear();
                    if (p.UserList != null)
                    {
                        foreach (var user in p.UserList)
                        {
                            lstOnlineUsers.Items.Add(user);
                        }
                    }
                    break;

                case PacketType.FileList:
                    _currentFileList = p.FileList;
                    DisplayFileList();
                    break;

                case PacketType.FileDownloadResponse:
                    if (p.FileData != null && p.FileData.Length > 0)
                    {
                        SaveReceivedFile(p.FileName, p.FileData);
                    }
                    break;
            }
        }

        private void AppendSystemMessage(string text, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.AppendText(text + "\n");
            rtbChat.SelectionColor = rtbChat.ForeColor;
        }

        private void AppendMessageWithColor(string text, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.AppendText(text);
            rtbChat.SelectionColor = rtbChat.ForeColor;
        }

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

        private void SaveReceivedFile(string? fileName, byte[] fileData)
        {
            try
            {
                string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string filePath = Path.Combine(downloadFolder, fileName ?? "received_file");

                // Prevent overwriting
                int counter = 1;
                string baseFileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(downloadFolder, $"{baseFileName}_{counter}{extension}");
                    counter++;
                }

                File.WriteAllBytes(filePath, fileData);
                AppendSystemMessage($"✅ File đã lưu: {filePath}", Color.Green);
            }
            catch (Exception ex)
            {
                AppendSystemMessage($"❌ Lỗi lưu file: {ex.Message}", Color.Red);
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUsername))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtInput.Text)) return;

            if (_currentGroupId >= 0)
            {
                // Send to group
                var p = new Packet(PacketType.GroupMessage)
                {
                    Sender = _currentUsername,
                    Content = txtInput.Text,
                    GroupId = _currentGroupId
                };
                await SendPacket(p);
            }
            else
            {
                // Send to all
                var p = new Packet(PacketType.Message)
                {
                    Sender = _currentUsername,
                    Content = txtInput.Text
                };
                await SendPacket(p);
            }

            AppendMessageWithColor($"Tôi: {txtInput.Text}\n", Color.Blue);
            txtInput.Clear();
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUsername))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file để gửi";
                ofd.Filter = "All Files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = ofd.FileName;
                        string fileName = Path.GetFileName(filePath);
                        byte[] fileData = File.ReadAllBytes(filePath);

                        var p = new Packet(PacketType.File)
                        {
                            Sender = _currentUsername,
                            FileName = fileName,
                            FileSize = fileData.Length,
                            FileData = fileData,
                            GroupId = _currentGroupId
                        };

                        await SendPacket(p);
                        AppendSystemMessage($"📤 Gửi file: {fileName} ({FormatFileSize(fileData.Length)})", Color.Purple);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi gửi file: {ex.Message}");
                    }
                }
            }
        }

        private async void btnDownloadFile_Click(object sender, EventArgs e)
        {
            if (lstFileList?.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn một file!");
                return;
            }

            if (_currentFileList == null || _currentFileList.Count == 0)
                return;

            var selectedFile = _currentFileList[lstFileList.SelectedIndex];
            var requestPacket = new Packet(PacketType.FileDownloadRequest) { FileId = selectedFile.Id };
            await SendPacket(requestPacket);
            AppendSystemMessage($"📥 Đang tải file: {selectedFile.FileName}...", Color.Blue);
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                btnSend_Click(sender, e);
            }
        }

        private void lstGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstGroups.SelectedIndex < 0)
            {
                btnJoinGroup.Enabled = false;
                btnLeaveGroup.Enabled = false;
                return;
            }

            // Enable buttons when a group is selected
            btnJoinGroup.Enabled = true;
            btnLeaveGroup.Enabled = (_currentGroupId == lstGroups.SelectedIndex);

            // Show which group is selected
            if (_currentGroupId == lstGroups.SelectedIndex)
            {
                btnJoinGroup.Text = "Đã tham gia";
                btnJoinGroup.Enabled = false;
                btnLeaveGroup.Enabled = true;
            }
            else
            {
                btnJoinGroup.Text = "Tham gia Nhóm";
                btnJoinGroup.Enabled = true;
                btnLeaveGroup.Enabled = false;
            }
        }

        private async void btnJoinGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUsername))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            if (lstGroups.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn một nhóm!");
                return;
            }

            int selectedGroupId = lstGroups.SelectedIndex;
            _currentGroupId = selectedGroupId;

            rtbChat.Clear(); // Clear chat when switching groups
            string groupName = _groupNames[selectedGroupId];

            // Load chat history for this group
            LoadGroupHistory(selectedGroupId);

            var p = new Packet(PacketType.JoinGroup)
            {
                GroupId = selectedGroupId,
                Sender = _currentUsername
            };
            await SendPacket(p);
            AppendSystemMessage($"✅ Tham gia nhóm: {groupName}", Color.Green);

            // Update button states
            btnJoinGroup.Text = "Đã tham gia";
            btnJoinGroup.Enabled = false;
            btnLeaveGroup.Enabled = true;
        }

        private async void btnLeaveGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUsername))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            if (_currentGroupId < 0)
            {
                MessageBox.Show("Bạn chưa tham gia nhóm nào!");
                return;
            }

            string groupName = _groupNames[_currentGroupId];
            int departingGroupId = _currentGroupId;
            _currentGroupId = -1;

            var p = new Packet(PacketType.LeaveGroup)
            {
                GroupId = departingGroupId,
                Sender = _currentUsername
            };
            await SendPacket(p);
            AppendSystemMessage($"✅ Rời khỏi nhóm: {groupName}", Color.Green);

            rtbChat.Clear(); // Clear chat when leaving group
            lstGroups.SelectedIndex = -1;

            // Update button states
            btnJoinGroup.Text = "Tham gia Nhóm";
            btnJoinGroup.Enabled = false;
            btnLeaveGroup.Enabled = false;
        }

        private async Task SendPacket(Packet p)
        {
            var d = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(p));
            await _stream!.WriteAsync(d, 0, d.Length);
        }

        private void RefreshFileList()
        {
            var fileListRequest = new Packet(PacketType.FileListRequest);
            _ = SendPacket(fileListRequest);
        }

        private void DisplayFileList()
        {
            if (lstFileList == null) return;
            lstFileList.Items.Clear();
            if (_currentFileList == null || _currentFileList.Count == 0)
            {
                lstFileList.Items.Add("(Không có file nào)");
                return;
            }
            foreach (var file in _currentFileList)
            {
                string displayText = $"{file.FileName} ({FormatFileSize(file.FileSize)}) - {file.UploadedTime:HH:mm:ss}";
                lstFileList.Items.Add(displayText);
            }
        }

        private void LoadGroupHistory(int groupId)
        {
            try
            {
                string historyFile = Path.Combine(AppContext.BaseDirectory, "chat_history.json");
                if (!File.Exists(historyFile))
                    return;

                var json = File.ReadAllText(historyFile);
                var allMessages = JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();

                var groupMessages = allMessages
                    .Where(m => m.GroupId == groupId)
                    .OrderBy(m => m.Timestamp)
                    .ToList();

                foreach (var msg in groupMessages)
                {
                    Color senderColor = msg.Sender == _currentUsername ? Color.Blue : Color.Red;
                    if (msg.Sender == "HỆ THỐNG") senderColor = Color.Green;

                    string timestamp = msg.Timestamp.ToString("HH:mm:ss");
                    AppendMessageWithColor($"[{timestamp}] {msg.Sender}: {msg.Content}\n", senderColor);
                }
            }
            catch { /* Ignore errors loading history */ }
        }

        private void lstFileList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rtbChat_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}