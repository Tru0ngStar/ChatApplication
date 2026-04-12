# 💬 Chat Application - Nâng Cấp Hoàn Chỉnh

Ứng dụng chat đa luồng được xây dựng bằng .NET 10 với giao diện WinForms, hỗ trợ chat toàn server, group chat, gửi/nhận file và xác thực người dùng.

## 🎯 Tính Năng Chính

### 1. 🔐 Xác Thực Người Dùng
- Đăng nhập & Đăng ký qua giao diện
- Lưu trữ tài khoản trong `accounts.txt`
- Xác thực username/password trên server

### 2. 💬 Chat Toàn Server
- Gửi tin nhắn cho tất cả user online
- Tin nhắn có màu sắc (tôi/người khác/hệ thống)
- Real-time messaging

### 3. 👥 Danh Sách User Online
- Hiển thị toàn bộ user đang trực tuyến
- Cập nhật tự động khi có user mới vào/ra

### 4. 📁 Gửi/Nhận File
- Chọn file qua OpenFileDialog
- Hỗ trợ file tối đa 10MB
- Tự động lưu vào folder Downloads
- Hiển thị dung lượng file (KB, MB, GB)

### 5. 🎭 Group Chat
- Tạo nhóm chat riêng
- Tham gia/Rời khỏi nhóm
- Tin nhắn nhóm chỉ gửi cho thành viên

### 6. ⚡ Multi-threading
- Hỗ trợ nhiều client kết nối đồng thời
- Async/await pattern
- Thread-safe operations

## 🏗️ Kiến Trúc

```
ChatApplication/
├── ChatShared/
│   └── Packet.cs              # Định nghĩa cấu trúc dữ liệu (8 loại packet)
├── ChatServer/
│   ├── Program.cs             # TCP Listener + Broadcast
│   ├── ClientHandler.cs       # Xử lý logic từng client
│   └── DatabaseContext.cs     # Quản lý accounts.txt
└── ChatClient/
    ├── Form1.cs               # Logic giao diện
    └── Form1.Designer.cs      # UI Controls
```

## 🚀 Bắt Đầu Nhanh

### Yêu Cầu
- .NET 10 SDK
- Visual Studio 2022+ hoặc VS Code

### Chạy Ứng Dụng

**1. Chạy Server**
```bash
cd ChatServer
dotnet run
```

Kỳ vọng:
```
======================================
 SERVER CHAT ĐÃ KHỞI ĐỘNG
 Cổng: 9999
 Thời gian: [Current Time]
======================================
```

**2. Chạy Client**
```bash
cd ChatClient
dotnet run
```

**3. Mở nhiều Client cùng lúc (tùy chọn)**
- Từ các cửa sổ terminal khác hoặc từ Visual Studio

## 📖 Hướng Dẫn Sử Dụng

### Đăng Nhập/Đăng Ký
```
1. Nhập username & password
2. Bấm "Đăng ký" để tạo tài khoản (lần đầu)
3. Bấm "Đăng nhập" để vào chat
4. Danh sách user online sẽ hiển thị
```

### Chat Toàn Server
```
1. Nhập tin nhắn trong ô nhập
2. Bấm "Gửi" hoặc Enter
3. Tất cả user online sẽ nhận được
```

### Gửi File
```
1. Bấm "Gửi File"
2. Chọn file từ máy tính
3. Các user khác sẽ nhận được notification
4. File tự động lưu vào Downloads
```

### Group Chat
```
1. Nhập tên nhóm (ví dụ: "Project_A")
2. Bấm "Tạo Nhóm" hoặc "Tham gia Nhóm"
3. Gửi tin nhắn - chỉ thành viên nhóm nhận được
4. Bấm "Rời Nhóm" để quay lại chat chung
```

## 📊 Cấu Trúc Packet

```csharp
public enum PacketType
{
    Register,        // Đăng ký tài khoản
    Login,          // Đăng nhập
    Message,        // Tin nhắn chung
    File,           // Gửi file
    UpdateUserList, // Cập nhật danh sách user
    CreateGroup,    // Tạo nhóm
    JoinGroup,      // Tham gia nhóm
    GroupMessage    // Tin nhắn nhóm
}
```

## 🔧 Tệp Cấu Hình

### accounts.txt
```
user1:password123
user2:securepass
admin:admin123
```

**Vị trí:** `ChatServer/bin/Debug/net10.0/accounts.txt`

## 📝 Công Nghệ Sử Dụng

- **Language:** C# 14
- **Framework:** .NET 10
- **UI:** Windows Forms
- **Networking:** TCP Sockets
- **Serialization:** System.Text.Json
- **Async:** Task-based async/await
- **Storage:** Plain text file

## 🎨 Giao Diện

### Màu Sắc Tin Nhắn
| Loại | Màu | Ví Dụ |
|------|-----|-------|
| Tin nhắn của tôi | 🔵 Blue | "Tôi: Xin chào" |
| Tin nhắn người khác | 🔴 Red | "Alice: Chào!" |
| Tin nhắn hệ thống | 🟢 Green | "HỆ THỐNG: User online" |
| Thông báo file | 🟣 Purple | "📁 Bob gửi file" |

### Layout
```
┌─────────────────┬──────────────────────────┐
│   Auth Form     │                          │
│  ├ Username     │      Chat Messages       │
│  ├ Password     │      (RichTextBox)       │
│  ├ Login        │                          │
│  └ Register     │                          │
├─────────────────┼──────────────────────────┤
│  Online Users   │  Message Input           │
│  ├ user1        │  ├ TextBox               │
│  ├ user2        │  ├ Send Button           │
│  └ user3        │  └ Send File Button      │
├─────────────────┤                          │
│  Group Chat     │                          │
│  ├ Group Name   │                          │
│  ├ Create Group │                          │
│  ├ Join Group   │                          │
│  └ Leave Group  │                          │
└─────────────────┴──────────────────────────┘
```

## ⚙️ Thiết Lập Nâng Cao

### Thay Đổi Port
File: `ChatServer/Program.cs`
```csharp
private const int Port = 9999; // Đổi số cổng ở đây
```

### Thay Đổi Server Address
File: `ChatClient/Form1.cs`
```csharp
_client = new TcpClient("127.0.0.1", 9999); // Đổi IP ở đây
```

### Tăng Buffer Size
File: `ChatServer/ClientHandler.cs` & `ChatClient/Form1.cs`
```csharp
byte[] buffer = new byte[10 * 1024 * 1024]; // 10MB → tăng số này
```

## 🐛 Khắc Phục Sự Cố

| Lỗi | Giải Pháp |
|-----|----------|
| "Server chưa bật" | Chắc chắn ChatServer đang chạy |
| Port 9999 đang sử dụng | Đóng ứng dụng cũ hoặc đổi port |
| File không lưu | Kiểm tra quyền truy cập Downloads folder |
| Tin nhắn không gửi được | Kiểm tra kết nối mạng |

## 📚 Tài Liệu Thêm

- **FEATURES.md** - Danh sách chi tiết các tính năng
- **TESTING_GUIDE.md** - Hướng dẫn kiểm tra từng tính năng

## 🎓 Bài Học

Project này minh họa:
- TCP Socket programming với async/await
- Multi-threading trong C#
- WinForms UI design
- JSON serialization
- Thread-safe operations

## 🚀 Hướng Phát Triển Tiếp Theo

- [ ] Hash password bằng SHA256/bcrypt
- [ ] Chuyển từ file → Database (SQL Server/MySQL)
- [ ] Direct messaging (1-to-1 chat)
- [ ] Lưu lịch sử chat
- [ ] WPF hoặc .NET MAUI UI
- [ ] Voice/Video calling
- [ ] Emoji & Reactions
- [ ] Typing indicator
- [ ] File compression
- [ ] Message search

## 📄 License

Dự án này được tạo cho mục đích học tập.

## 👨‍💻 Tác Giả

Phát triển bởi: Chat Application Team

---

**Phiên bản:** 2.0 (Nâng cấp hoàn chỉnh)  
**Ngày cập nhật:** 2024  
**Status:** ✅ Hoàn thành tất cả 6 yêu cầu

---

## ✨ Highlight Chính

✅ **Giao thức:**  8 loại packet được định nghĩa rõ ràng  
✅ **Auth:**  accounts.txt với xác thực đơn giản nhưng hiệu quả  
✅ **Threading:**  Multi-threading tự nhiên với async/await  
✅ **File Transfer:**  Hỗ trợ upload/download file 10MB  
✅ **Group Chat:**  Hệ thống nhóm hoàn chỉnh  
✅ **UI:**  Giao diện cải thiện với danh sách user, màu sắc, file dialog  

🎉 **Tất cả yêu cầu đã hoàn thành!**
