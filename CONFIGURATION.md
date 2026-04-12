# ⚙️ Configuration Guide

## 🔧 Cấu Hình Ứng Dụng

Tất cả các cấu hình chính nằm ở các hằng số trong code. Bạn có thể dễ dàng thay đổi chúng:

---

## 🖥️ Server Configuration

### File: `ChatServer/Program.cs`

#### 1. Thay Đổi Cổng

```csharp
private const int Port = 9999; // ← Đổi số cổng ở đây
```

**Ví dụ:**
```csharp
private const int Port = 8080;  // Cổng 8080
private const int Port = 5000;  // Cổng 5000
```

**Lưu ý:** Sau khi thay đổi, client cũng phải cập nhật cùng cổng!

---

#### 2. Thay Đổi Server Address

```csharp
_listener = new TcpListener(IPAddress.Any, Port);
// IPAddress.Any = chấp nhận kết nối từ bất kỳ IP nào
```

**Để chỉ chấp nhận localhost:**
```csharp
_listener = new TcpListener(IPAddress.Loopback, Port);
```

**Để chỉ chấp nhận IP cụ thể:**
```csharp
_listener = new TcpListener(IPAddress.Parse("192.168.1.100"), Port);
```

---

### File: `ChatServer/ClientHandler.cs`

#### 1. Thay Đổi Buffer Size

```csharp
byte[] buffer = new byte[10 * 1024 * 1024]; // 10MB
```

**Giá trị gợi ý:**
- 1MB = `1 * 1024 * 1024` (file nhỏ)
- 10MB = `10 * 1024 * 1024` (file trung bình)
- 50MB = `50 * 1024 * 1024` (file lớn)
- 100MB = `100 * 1024 * 1024` (file rất lớn)

**Lưu ý:** Buffer càng lớn, càng tiêu tốn RAM!

---

#### 2. Thay Đổi Timeout (Optional)

Bạn có thể thêm timeout cho socket:

```csharp
public ClientHandler(TcpClient client)
{
    _client = client;
    _client.ReceiveTimeout = 30000; // 30 seconds
    _client.SendTimeout = 30000;    // 30 seconds
    _stream = client.GetStream();
}
```

---

### File: `ChatServer/DatabaseContext.cs`

#### 1. Thay Đổi Vị Trí File accounts.txt

```csharp
private static readonly string AccountsFile = 
    Path.Combine(AppContext.BaseDirectory, "accounts.txt");
```

**Để lưu ở Desktop:**
```csharp
private static readonly string AccountsFile = 
    Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.Desktop), "accounts.txt");
```

**Để lưu ở đường dẫn cụ thể:**
```csharp
private static readonly string AccountsFile = 
    @"C:\Users\YourUsername\Documents\accounts.txt";
```

---

#### 2. Thêm Mã Hóa Mật Khẩu (Advanced)

```csharp
using System.Security.Cryptography;
using System.Text;

private static string HashPassword(string password)
{
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

public static bool RegisterUser(string user, string pass)
{
    // ... check duplicate ...
    string hashedPass = HashPassword(pass);
    File.AppendAllLines(AccountsFile, new[] { $"{user}:{hashedPass}" });
    return true;
}

public static bool ValidateUser(string user, string pass)
{
    string hashedPass = HashPassword(pass);
    // ... so sánh hashedPass với file ...
}
```

---

## 💻 Client Configuration

### File: `ChatClient/Form1.cs`

#### 1. Thay Đổi Server Address & Port

```csharp
private async Task<bool> Connect()
{
    try
    {
        _client = new TcpClient("127.0.0.1", 9999);
        //                       ^^^^^^^^^^  ^^^^
        //                       IP address  Port
        _stream = _client.GetStream();
        _ = Task.Run(Receive);
        return true;
    }
    catch { MessageBox.Show("Server chưa bật!"); return false; }
}
```

**Để kết nối tới server khác:**
```csharp
_client = new TcpClient("192.168.1.100", 9999);  // IP khác
_client = new TcpClient("127.0.0.1", 8080);      // Port khác
```

---

#### 2. Thay Đổi Buffer Size

```csharp
byte[] buffer = new byte[10 * 1024 * 1024]; // 10MB
```

**Lưu ý:** Phải khớp với server buffer size!

---

#### 3. Thay Đổi Downloads Folder

```csharp
private void SaveReceivedFile(string? fileName, byte[] fileData)
{
    try
    {
        // Hiện tại: Downloads folder
        string downloadFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
            "Downloads");
        
        // Để thay đổi:
        // string downloadFolder = "C:\\MyReceivedFiles";
        // string downloadFolder = Environment.GetFolderPath(
        //     Environment.SpecialFolder.Desktop);
        
        string filePath = Path.Combine(downloadFolder, fileName ?? "received_file");
        // ...
    }
}
```

---

#### 4. Thay Đổi Màu Sắc Tin Nhắn

```csharp
case PacketType.Message:
    Color senderColor = p.Sender == _currentUsername ? Color.Blue : Color.Red;
    if (p.Sender == "HỆ THỐNG") senderColor = Color.Green;
    AppendMessageWithColor($"{p.Sender}: {p.Content}\n", senderColor);
    break;
```

**Thay đổi màu:**
```csharp
// Tin nhắn của tôi: xanh dương → vàng
Color senderColor = p.Sender == _currentUsername ? Color.Gold : Color.Red;

// Tin nhắn người khác: đỏ → tím
Color senderColor = p.Sender == _currentUsername ? Color.Blue : Color.Magenta;

// Hệ thống: xanh lá → cyan
if (p.Sender == "HỆ THỐNG") senderColor = Color.Cyan;
```

---

#### 5. Thay Đổi Kích Thước Form

File: `ChatClient/Form1.Designer.cs`

```csharp
ClientSize = new Size(950, 570); // Rộng x Cao (pixel)
```

**Ví dụ:**
```csharp
ClientSize = new Size(1200, 700);  // Form lớn hơn
ClientSize = new Size(800, 480);   // Form nhỏ hơn
```

---

## 🎨 Cấu Hình UI

### Thay Đổi Font

```csharp
// Trong Form1.Designer.cs hoặc Form1_Load
rtbChat.Font = new Font("Arial", 12); // Font Arial, size 12
txtInput.Font = new Font("Courier New", 10);
```

### Thay Đổi Control Styles

```csharp
// Làm cho TextBox multi-line
txtInput.Multiline = true;
txtInput.Height = 60;

// Thêm scrollbar cho RichTextBox
rtbChat.ScrollBars = RichTextBoxScrollBars.Both;
```

---

## 📝 Cấu Hình Protocol

### File: `ChatShared/Packet.cs`

Bạn có thể thêm packet types mới:

```csharp
public enum PacketType
{
    Register,
    Login,
    Message,
    File,
    UpdateUserList,
    CreateGroup,
    JoinGroup,
    GroupMessage,
    // Thêm loại mới:
    DirectMessage,    // Tin nhắn riêng
    TypingIndicator,  // Đang gõ...
    UserStatus,       // Trạng thái online/offline
    Notification      // Thông báo chung
}
```

---

## ⚡ Performance Tuning

### 1. Tối Ưu Receive Loop

```csharp
// Trước: Đợi full buffer
int read = await _stream.ReadAsync(buffer, 0, buffer.Length);

// Sau: Read thường xuyên hơn
int read = await _stream.ReadAsync(buffer, 0, 64 * 1024); // 64KB
```

### 2. Tối Ưu Message Parsing

```csharp
// Thay vì split string, dùng regex hoặc tách từng byte
var packets = json.Replace("}{", "}|{").Split('|');
foreach (var p in packets)
{
    if (string.IsNullOrWhiteSpace(p)) continue; // Bỏ qua rỗng
    // ...
}
```

### 3. Giới Hạn Message History

```csharp
if (rtbChat.Lines.Length > 500) // Nếu > 500 dòng
{
    // Xóa 100 dòng đầu tiên
    int removeLines = 100;
    rtbChat.Select(0, rtbChat.Lines
        .Take(removeLines)
        .Sum(l => l.Length + 1));
    rtbChat.SelectedText = "";
}
```

---

## 🔐 Security Configuration

### 1. Thêm Input Validation

```csharp
private bool IsValidUsername(string username)
{
    // Username phải 3-20 ký tự, chỉ a-z, 0-9, _
    return Regex.IsMatch(username, @"^[a-z0-9_]{3,20}$", 
        RegexOptions.IgnoreCase);
}

private bool IsValidPassword(string password)
{
    // Password phải >= 6 ký tự
    return password?.Length >= 6;
}
```

### 2. Rate Limiting

```csharp
private Dictionary<string, DateTime> _loginAttempts = new();

private bool CheckRateLimit(string username)
{
    if (!_loginAttempts.ContainsKey(username)) return true;
    
    var lastAttempt = _loginAttempts[username];
    if ((DateTime.Now - lastAttempt).TotalSeconds < 5)
        return false; // Quá nhanh
    
    return true;
}
```

---

## 📊 Configuration Summary

### Các File Chính Cần Thay Đổi

| Cấu Hình | File | Dòng |
|---------|------|------|
| Port | `Program.cs` | `const int Port = ...` |
| Server IP | `Form1.cs` | `new TcpClient("...", ...)` |
| Buffer size | `ClientHandler.cs`, `Form1.cs` | `new byte[...]` |
| Accounts file | `DatabaseContext.cs` | `AccountsFile = ...` |
| Downloads folder | `Form1.cs` | `downloadFolder = ...` |
| Màu sắc | `Form1.cs` | `Color.Blue`, `Color.Red`, ... |

---

## ✅ Checklist Cấu Hình

- [ ] Port server & client khớp nhau
- [ ] Buffer size đủ cho file size tối đa
- [ ] Accounts file path tồn tại và có quyền write
- [ ] Downloads folder tồn tại
- [ ] Firewall cho phép port
- [ ] .NET 10 đã cài đặt

---

**Ghi chú:** Sau mỗi thay đổi cấu hình, phải **rebuild** project!

```bash
dotnet clean
dotnet build
```
