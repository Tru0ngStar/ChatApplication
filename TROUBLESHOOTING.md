# 🔧 Hướng Dẫn Khắc Phục Sự Cố

## 🚨 Sự Cố & Giải Pháp

### 1. ❌ "System.Net.Sockets.SocketException: Address already in use"

**Nguyên nhân:** Port 9999 đang bị sử dụng bởi ứng dụng khác

**Giải pháp:**

**Windows:**
```powershell
# Tìm process đang sử dụng port 9999
netstat -ano | findstr :9999

# Nếu tìm thấy, kill process
taskkill /PID <ProcessID> /F

# Hoặc đổi port trong Program.cs
private const int Port = 10000; // Thay 9999 bằng 10000
```

**macOS/Linux:**
```bash
# Tìm process
lsof -i :9999

# Kill process
kill -9 <PID>
```

---

### 2. ❌ "Unable to connect to the remote server"

**Nguyên nhân:** ChatServer chưa chạy hoặc địa chỉ IP sai

**Giải pháp:**

1. **Kiểm tra Server đang chạy:**
   - Mở ChatServer trước
   - Chắc chắn rằng console hiển thị: "SERVER CHAT ĐÃ KHỞI ĐỘNG"

2. **Kiểm tra địa chỉ IP (Form1.cs):**
   ```csharp
   // Nếu chạy trên máy tính khác:
   _client = new TcpClient("192.168.1.100", 9999); // Thay "127.0.0.1"
   
   // Nếu chạy trên cùng máy:
   _client = new TcpClient("127.0.0.1", 9999); // Giữ nguyên
   ```

3. **Kiểm tra Firewall:**
   - Windows Defender có thể block port 9999
   - Mở Windows Defender → Firewall → Allow app through firewall
   - Thêm `dotnet.exe` hoặc ứng dụng của bạn

---

### 3. ❌ "accounts.txt not found" hoặc không lưu được

**Nguyên nhân:** File được tạo trong thư mục sai hoặc quyền không đủ

**Giải pháp:**

1. **Kiểm tra vị trí file:**
   ```
   Nên ở: ChatServer/bin/Debug/net10.0/accounts.txt
   ```

2. **Tạo file thủ công:**
   ```
   Tạo file "accounts.txt" với nội dung rỗng
   Đặt trong: ChatServer/bin/Debug/net10.0/
   ```

3. **Cấp quyền write:**
   - Right-click folder → Properties → Security
   - Đảm bảo user hiện tại có quyền write

---

### 4. ❌ "Đăng nhập thất bại" dù username/password đúng

**Nguyên nhân:** Username bị case-sensitive hoặc ký tự không khớp

**Giải pháp:**

1. **Kiểm tra file accounts.txt:**
   ```
   Mở: ChatServer/bin/Debug/net10.0/accounts.txt
   Xem: user1:pass123
   ```

2. **So sánh từng ký tự:**
   - Không có khoảng trắng thừa
   - Chính xác từ username đến password

3. **Xóa & Đăng ký lại:**
   ```
   Xóa dòng cũ khỏi accounts.txt
   Đăng ký lại với username/password chính xác
   ```

---

### 5. ❌ Danh sách user online không cập nhật

**Nguyên nhân:** Client chưa đăng nhập hoặc Server chưa gửi update

**Giải pháp:**

1. **Đăng nhập lại:**
   - Đứng ra và đăng nhập lại
   - ListBox sẽ cập nhật

2. **Kiểm tra Server logs:**
   - Xem có dòng: "[AUTH] user1 đã đăng nhập thành công"
   - Nếu không thì xác thực thất bại

3. **Kiểm tra Packet mạng:**
   - Thêm dòng debug: `Console.WriteLine(packet.UserList?.Count)` trong server

---

### 6. ❌ File không gửi được hoặc bị lỗi

**Nguyên nhân:** Buffer quá nhỏ, file quá lớn, hoặc exception không bắt

**Giải pháp:**

1. **Tăng buffer size:**
   ```csharp
   // ChatServer/ClientHandler.cs
   byte[] buffer = new byte[50 * 1024 * 1024]; // 50MB
   
   // ChatClient/Form1.cs
   byte[] buffer = new byte[50 * 1024 * 1024]; // 50MB
   ```

2. **Kiểm tra kích thước file:**
   - File phải < buffer size
   - Ví dụ: nếu buffer 10MB, file <= 10MB

3. **Xem error trong client:**
   ```
   MessageBox sẽ hiển thị: "Lỗi khi gửi file: ..."
   Theo dõi vị trí file được lưu
   ```

4. **Kiểm tra Downloads folder:**
   - Mở: C:\Users\[YourUsername]\Downloads
   - Tìm file với tên: "filename.ext" hoặc "filename_1.ext"

---

### 7. ❌ Group chat không hoạt động

**Nguyên nhân:** Tên nhóm khác nhau, chưa join, hoặc logic lỗi

**Giải pháp:**

1. **Kiểm tra tên nhóm chính xác:**
   ```
   Cả hai client phải nhập tên nhóm GIỐNG nhau
   Ví dụ: "Project_A" ≠ "project_a" (case-sensitive)
   ```

2. **Thử tạo nhóm mới:**
   - Một client: "Tạo Nhóm" với tên "TestGroup"
   - Client khác: "Tham gia Nhóm" với tên "TestGroup"

3. **Kiểm tra Server logs:**
   ```
   Kỳ vọng thấy:
   [GROUP] user1 tham gia nhóm: TestGroup
   [GROUP] user2 tham gia nhóm: TestGroup
   ```

4. **Gửi tin nhắn kiểm tra:**
   - Gửi từ một client trong nhóm
   - Kiểm tra xem các client khác trong nhóm có nhận không

---

### 8. ❌ Build Error: "Project not found"

**Nguyên nhân:** Đường dẫn sai hoặc file .csproj bị xóa

**Giải pháp:**

1. **Kiểm tra cấu trúc thư mục:**
   ```
   ChatApplication/
   ├── ChatShared/ChatShared.csproj
   ├── ChatServer/ChatServer.csproj
   ├── ChatClient/ChatClient.csproj
   └── .sln file
   ```

2. **Build từ thư mục gốc:**
   ```powershell
   cd E:\Bài tập lớn lập trình mạng\repo2\
   dotnet build
   ```

3. **Xóa cache & rebuild:**
   ```powershell
   dotnet clean
   dotnet build
   ```

---

### 9. ❌ "JsonException: The JSON value could not be converted"

**Nguyên nhân:** Dữ liệu JSON không hợp lệ hoặc format sai

**Giải pháp:**

1. **Kiểm tra serialization:**
   ```csharp
   var json = JsonSerializer.Serialize(packet);
   Console.WriteLine(json); // Xem JSON output
   ```

2. **Kiểm tra deserialize:**
   ```csharp
   try {
       var p = JsonSerializer.Deserialize<Packet>(json);
   } catch (JsonException ex) {
       Console.WriteLine($"Error: {ex.Message}");
       Console.WriteLine($"JSON: {json}");
   }
   ```

3. **Xóa `null` bytes:**
   ```csharp
   json = Encoding.UTF8.GetString(buffer, 0, n).TrimEnd('\0');
   ```

---

### 10. ❌ Memory leak / Ứng dụng ngày càng chậm

**Nguyên nhân:** Connection không close, lưu quá nhiều dữ liệu, hoặc event không unsubscribe

**Giải pháp:**

1. **Kiểm tra khi disconnect:**
   ```csharp
   public void Disconnect() 
   {
       _client?.Close();      // Đóng connection
       _stream?.Dispose();    // Giải phóng stream
       _client?.Dispose();    // Giải phóng client
   }
   ```

2. **Giới hạn message history:**
   ```csharp
   if (rtbChat.Lines.Length > 1000) {
       rtbChat.Clear();
   }
   ```

3. **Unsubscribe events:**
   - Nếu dùng events, phải unsubscribe khi thoát

---

### 11. ❌ "Operation is not valid due to the current state of the object"

**Nguyên nhân:** Access UI từ thread khác mà không dùng `Invoke`

**Giải pháp:**

```csharp
// ❌ Sai:
rtbChat.AppendText("Message");

// ✅ Đúng:
this.Invoke(() => {
    rtbChat.AppendText("Message");
});
```

---

### 12. ❌ Hiệu suất thấp / Delay khi chat

**Nguyên nhân:** Network chậm, server bận, hoặc logic không tối ưu

**Giải pháp:**

1. **Kiểm tra latency:**
   ```powershell
   ping 127.0.0.1  # Localhost - nên < 1ms
   ping [ServerIP] # Remote - nên < 100ms
   ```

2. **Tối ưu hóa:**
   - Giảm buffer size (nếu không gửi file lớn)
   - Dùng `Span<T>` thay vì array nếu cần
   - Profile code với Visual Studio Profiler

3. **Kiểm tra Server CPU/RAM:**
   - Task Manager → Performance
   - Xem có spike không

---

## 📊 Bảng Chẩn Đoán Nhanh

| Lỗi | Nguyên Nhân | Cách Kiểm Tra | Fix |
|-----|-----------|-------------|-----|
| Can't connect | Server off | `ping 127.0.0.1` | Chạy server |
| Port in use | Ứng dụng khác | `netstat -ano \| findstr :9999` | Kill process |
| Auth fail | Sai username | Mở `accounts.txt` | Đăng ký lại |
| File error | File quá lớn | Kiểm tra byte[] length | Tăng buffer |
| Group fail | Tên nhóm khác | Kiểm tra console log | Nhập tên chính xác |
| Memory leak | Resource không release | Task Manager | `using` statement |
| JSON error | Format sai | `Console.WriteLine(json)` | Kiểm tra format |
| UI lag | Thread blocking | Dùng `async/await` | Code restructure |

---

## 🔍 Debug Mode

### Bật Debug Logging

**ChatServer/Program.cs:**
```csharp
#if DEBUG
    Console.WriteLine($"[DEBUG] Client connected: {handler.Username}");
    Console.WriteLine($"[DEBUG] Packet received: {JsonSerializer.Serialize(packet)}");
#endif
```

**ChatClient/Form1.cs:**
```csharp
#if DEBUG
    Console.WriteLine($"[DEBUG] Message from: {p.Sender}");
    Console.WriteLine($"[DEBUG] Packet type: {p.Type}");
#endif
```

---

## 📞 Liên Hệ Hỗ Trợ

Nếu sự cố không được giải quyết:

1. **Kiểm tra lại các bước:**
   - Đã build thành công?
   - Có .NET 10 không?
   - Firewall có block không?

2. **Xem logs:**
   - Server console output
   - Client exception messages
   - Event Viewer (Windows)

3. **Thử lại:**
   - Clean + Rebuild
   - Restart ứng dụng
   - Restart máy tính

---

✅ **Hầu hết vấn đề có thể được giải quyết bằng các bước trên!**
