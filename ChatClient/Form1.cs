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

        public Form1() { InitializeComponent(); }

        private async void btnLogin_Click(object sender, EventArgs e) => await Auth(PacketType.Login);
        private async void btnRegister_Click(object sender, EventArgs e) => await Auth(PacketType.Register);

        private async Task Auth(PacketType type)
        {
            if (!await Connect()) return;
            var p = new Packet(type) { Sender = txtUsername.Text, Password = txtPassword.Text };
            await SendPacket(p);
        }

        private async Task<bool> Connect()
        {
            if (_client?.Connected == true) return true;
            try
            {
                _client = new TcpClient("127.0.0.1", 9999);
                _stream = _client.GetStream();
                _ = Task.Run(Receive);
                return true;
            }
            catch { MessageBox.Show("Server chưa bật!"); return false; }
        }

        private async Task Receive()
        {
            byte[] buffer = new byte[1024 * 1024];
            while (_client?.Connected == true)
            {
                int n = await _stream!.ReadAsync(buffer, 0, buffer.Length);
                if (n == 0) break;
                string json = Encoding.UTF8.GetString(buffer, 0, n);
                var parts = json.Replace("}{", "}|{").Split('|');

                foreach (var part in parts)
                {
                    var p = JsonSerializer.Deserialize<Packet>(part);
                    if (p == null) continue;
                    this.Invoke(() => {
                        if (p.Type == PacketType.Login && p.IsSuccess)
                        {
                            _currentUsername = txtUsername.Text;
                            grpChat.Enabled = true; grpAuth.Enabled = false;
                            rtbChat.AppendText("--- Hệ thống: Đăng nhập thành công! ---\n");
                        }
                        else if (p.Type == PacketType.Message)
                        {
                            rtbChat.AppendText($"{p.Sender}: {p.Content}\n");
                        }
                        else if (!string.IsNullOrEmpty(p.Content)) MessageBox.Show(p.Content);
                    });
                }
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text)) return;
            var p = new Packet(PacketType.Message) { Sender = _currentUsername, Content = txtInput.Text };
            await SendPacket(p);
            rtbChat.AppendText($"Tôi: {txtInput.Text}\n");
            txtInput.Clear();
        }

        private async Task SendPacket(Packet p)
        {
            var d = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(p));
            await _stream!.WriteAsync(d, 0, d.Length);
        }
    }
}