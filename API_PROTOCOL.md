# 📡 API Protocol Documentation

## 🔌 TCP Socket Protocol

### Tổng Quan

Ứng dụng chat sử dụng **TCP Socket** để giao tiếp với server. Tất cả dữ liệu được truyền dưới dạng **JSON** được serialized từ class `Packet`.

---

## 📦 Packet Structure

### Base Packet

```csharp
public class Packet
{
    public PacketType Type { get; set; }           // Loại packet
    public string? Sender { get; set; }            // Người gửi
    public string? Password { get; set; }          // Mật khẩu (chỉ dùng cho Register/Login)
    public string? Content { get; set; }           // Nội dung tin nhắn
    public bool IsSuccess { get; set; }            // Kết quả (true/false)
    public string? FileName { get; set; }          // Tên file
    public long FileSize { get; set; }             // Dung lượng file
    public byte[]? FileData { get; set; }          // Dữ liệu file (binary)
    public string? GroupName { get; set; }         // Tên nhóm
    public List<string>? UserList { get; set; }    // Danh sách user
}
```

### PacketType Enum

```csharp
public enum PacketType
{
    Register,        // ID: 0 - Đăng ký tài khoản
    Login,           // ID: 1 - Đăng nhập
    Message,         // ID: 2 - Tin nhắn chung (broadcast)
    File,            // ID: 3 - Gửi/nhận file
    UpdateUserList,  // ID: 4 - Cập nhật danh sách user
    CreateGroup,     // ID: 5 - Tạo nhóm
    JoinGroup,       // ID: 6 - Tham gia nhóm
    GroupMessage     // ID: 7 - Tin nhắn nhóm
}
```

---

## 📨 Message Flow

### 1️⃣ Register (Đăng Ký)

**Client → Server:**
```json
{
  "Type": 0,
  "Sender": "user1",
  "Password": "pass123",
  "Content": null,
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": null,
  "UserList": null
}
```

**Server → Client (Response):**
```json
{
  "Type": 0,
  "Sender": null,
  "Password": null,
  "Content": "Thành công!",
  "IsSuccess": true,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": null,
  "UserList": null
}
```

**Hoặc (nếu thất bại):**
```json
{
  "Type": 0,
  "Content": "Tài khoản tồn tại!",
  "IsSuccess": false
}
```

**Server Action:**
- Kiểm tra username có tồn tại trong `accounts.txt` không
- Nếu không tồn tại → Thêm vào file (định dạng: `username:password`)
- Phản hồi `IsSuccess = true/false`

---

### 2️⃣ Login (Đăng Nhập)

**Client → Server:**
```json
{
  "Type": 1,
  "Sender": "user1",
  "Password": "pass123",
  "Content": null,
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": null,
  "UserList": null
}
```

**Server → Client (Response):**
```json
{
  "Type": 1,
  "Content": null,
  "IsSuccess": true
}
```

**Server Action:**
- Kiểm tra username:password có hợp lệ trong `accounts.txt`
- Nếu hợp lệ → `IsSuccess = true` + gửi `UpdateUserList` cho tất cả client
- Nếu không → `IsSuccess = false`

---

### 3️⃣ Message (Tin Nhắn Chung)

**Client → Server:**
```json
{
  "Type": 2,
  "Sender": "user1",
  "Password": null,
  "Content": "Chào mọi người!",
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": null,
  "UserList": null
}
```

**Server → All Clients (Broadcast):**
```json
{
  "Type": 2,
  "Sender": "user1",
  "Content": "Chào mọi người!",
  "IsSuccess": false
}
```

**Server Action:**
- Kiểm tra Sender đã đăng nhập (Username != null)
- Broadcast packet này cho tất cả client khác
- Client gốc nhận lại nó cũng (hoặc exclude nó)

---

### 4️⃣ File (Gửi File)

**Client → Server:**
```json
{
  "Type": 3,
  "Sender": "user1",
  "Password": null,
  "Content": null,
  "IsSuccess": false,
  "FileName": "document.pdf",
  "FileSize": 2097152,
  "FileData": [137, 80, 78, 71, ...],
  "GroupName": null,
  "UserList": null
}
```

**Lưu ý:** `FileData` là mảng byte (binary data), được serialized thành base64 trong JSON

**Server → All Clients (Broadcast):**
```json
{
  "Type": 3,
  "Sender": "user1",
  "FileName": "document.pdf",
  "FileSize": 2097152,
  "FileData": [137, 80, 78, 71, ...],
  "IsSuccess": false
}
```

**Client Action (khi nhận):**
- Lấy `FileData` byte array
- Lấy `FileName`
- Lưu vào: `C:\Users\[User]\Downloads\{FileName}`
- Nếu file trùng → Thêm số vào tên: `document_1.pdf`

---

### 5️⃣ UpdateUserList (Cập Nhật Danh Sách User)

**Server → All Clients (Broadcast sau mỗi Login/Logout):**
```json
{
  "Type": 4,
  "Sender": null,
  "Password": null,
  "Content": null,
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": null,
  "UserList": ["user1", "user2", "user3"]
}
```

**Client Action:**
- Cập nhật ListBox `lstOnlineUsers`
- Xóa tất cả items cũ
- Thêm từng user trong `UserList`

**Gửi khi nào:**
- Sau mỗi successful login
- Sau mỗi client disconnect

---

### 6️⃣ CreateGroup (Tạo Nhóm)

**Client → Server:**
```json
{
  "Type": 5,
  "Sender": "user1",
  "Password": null,
  "Content": null,
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": "Project_A",
  "UserList": null
}
```

**Server → Group Members (Broadcast):**
```json
{
  "Type": 2,
  "Sender": "HỆ THỐNG",
  "Content": "user1 đã tạo nhóm: Project_A",
  "IsSuccess": false
}
```

**Server Action:**
- Tạo entry mới trong `_groups` dictionary: `_groups["Project_A"] = new List<ClientHandler>()`
- Thêm client hiện tại vào list
- Gửi tin nhắn hệ thống

---

### 7️⃣ JoinGroup (Tham Gia Nhóm)

**Client → Server:**
```json
{
  "Type": 6,
  "Sender": "user2",
  "Password": null,
  "Content": null,
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": "Project_A",
  "UserList": null
}
```

**Server → Group Members (Broadcast):**
```json
{
  "Type": 2,
  "Sender": "HỆ THỐNG",
  "Content": "user2 đã tham gia nhóm: Project_A",
  "IsSuccess": false
}
```

**Server Action:**
- Kiểm tra group tồn tại
- Thêm client vào `_groups["Project_A"]`
- Gửi tin nhắn hệ thống cho thành viên nhóm

---

### 8️⃣ GroupMessage (Tin Nhắn Nhóm)

**Client → Server:**
```json
{
  "Type": 7,
  "Sender": "user1",
  "Password": null,
  "Content": "Chat nhóm Project_A",
  "IsSuccess": false,
  "FileName": null,
  "FileSize": 0,
  "FileData": null,
  "GroupName": "Project_A",
  "UserList": null
}
```

**Server → Group Members Only (Broadcast):**
```json
{
  "Type": 2,
  "Sender": "user1",
  "Content": "Chat nhóm Project_A",
  "IsSuccess": false
}
```

**Server Action:**
- Chỉ gửi cho thành viên trong `_groups["Project_A"]`
- Không gửi cho những user khác nhóm

---

## 🔄 Connection Lifecycle

```
┌─────────────────┐
│  Client Start   │
└────────┬────────┘
         │
         ├─→ [Register] → Server → Response
         │
         ├─→ [Login] → Server → Response
         │              │
         │              └─→ Broadcast [UpdateUserList]
         │
    Connected!
         │
    ┌────┴─────────────────────────────────┐
    │                                       │
    ├─→ [Message] → Broadcast to all      │
    │                                       │
    ├─→ [File] → Broadcast to all         │
    │                                       │
    ├─→ [CreateGroup] → Broadcast         │
    │                                       │
    ├─→ [JoinGroup] → Broadcast           │
    │                                       │
    ├─→ [GroupMessage] → Broadcast in group
    │                                       │
    └─────────────────────────────────────┘
         │
    Disconnect/Close
         │
         └─→ Remove from _clients list
         └─→ Remove from groups
         └─→ Broadcast [UpdateUserList]
```

---

## 📊 Network Format

### Serialization Format

**Single Packet:**
```
{
  "Type": 2,
  "Sender": "user1",
  ...
}
```

**Multiple Packets (in same send):**
```
{"Type":2,"Sender":"user1",...}|{"Type":2,"Sender":"user2",...}
```

**Parsing Logic:**
```csharp
json = Encoding.UTF8.GetString(buffer, 0, read);
var packets = json.Replace("}{", "}|{").Split('|');
foreach (var p in packets) {
    var packet = JsonSerializer.Deserialize<Packet>(p);
    // Process packet
}
```

---

## ⚠️ Error Handling

### Common Errors

| Error | Packet.IsSuccess | Packet.Content |
|-------|------------------|-----------------|
| Register failed | false | "Tài khoản tồn tại!" |
| Login failed | false | (empty) |
| User not found | false | (empty) |
| File too large | false | (exception message) |

### Exception Handling

**Client side:**
```csharp
try {
    var p = JsonSerializer.Deserialize<Packet>(json);
    if (p != null && !p.IsSuccess) {
        MessageBox.Show(p.Content ?? "Lỗi: Unknown error");
    }
} catch (Exception ex) {
    MessageBox.Show($"Error: {ex.Message}");
}
```

**Server side:**
```csharp
try {
    await ProcessPacketAsync(packet);
} catch (Exception ex) {
    Console.WriteLine($"[ERROR] {ex.Message}");
    // Gửi error response
}
```

---

## 🔐 Security Notes

⚠️ **Hiện tại:**
- Password gửi plain text (không mã hóa)
- Không có authentication token
- Không có SSL/TLS encryption

✅ **Để cải thiện:**
```csharp
// 1. Hash password trước khi gửi
string hashedPass = SHA256.ComputeHash(password);

// 2. Thêm authentication token
public class Packet {
    public string? AuthToken { get; set; }
}

// 3. Sử dụng SSL/TLS
var sslStream = new SslStream(_stream);
sslStream.AuthenticateAsServer(certificate);
```

---

## 📈 Performance Metrics

### Message Size

| Packet Type | Avg Size | Notes |
|------------|----------|-------|
| Register/Login | ~100 bytes | username + password |
| Message | 50-500 bytes | Nội dung tin nhắn |
| File | 1MB-10MB+ | Tùy kích thước file |
| UpdateUserList | 100-1000 bytes | 10-100 users |

### Throughput

- **LAN (localhost):** < 1ms latency
- **LAN (same network):** 10-50ms latency
- **Internet:** 100-500ms latency
- **Max concurrent clients:** ~100+ (tùy server specs)

---

## 🧪 Testing Examples

### cURL Test (Windows PowerShell)

```powershell
# Kết nối TCP
$socket = New-Object System.Net.Sockets.TcpClient
$socket.Connect("127.0.0.1", 9999)

# Tạo packet
$packet = @{
    Type = 1
    Sender = "testuser"
    Password = "testpass"
} | ConvertTo-Json

# Gửi
$buffer = [Text.Encoding]::UTF8.GetBytes($packet)
$socket.GetStream().Write($buffer, 0, $buffer.Length)

# Nhận response
$reader = New-Object System.IO.StreamReader($socket.GetStream())
$response = $reader.ReadLine()
Write-Host $response
```

### Unit Test Example

```csharp
[Test]
public void TestLoginSuccess()
{
    var packet = new Packet(PacketType.Login) {
        Sender = "testuser",
        Password = "testpass"
    };
    
    bool isValid = DatabaseContext.ValidateUser(
        packet.Sender, packet.Password);
    
    Assert.IsTrue(isValid);
}
```

---

**Documentation Version:** 1.0  
**Last Updated:** 2024  
**Protocol Version:** 1.0 (TCP + JSON)
