# 💬 Chat Application - Hệ thống Chat Client-Server

Hệ thống Chat thời gian thực (Real-time) được xây dựng trên nền tảng .NET (C#), sử dụng kiến trúc Client-Server với giao thức TCP/IP. Dự án tích hợp cơ sở dữ liệu SQLite để quản lý người dùng.

## 🚀 Tính năng hiện tại
- **Đăng ký/Đăng nhập**: Lưu trữ thông tin tài khoản an toàn vào SQLite database.
- **Chat công khai (Broadcast)**: Client có thể gửi tin nhắn và toàn bộ các client đang online khác sẽ nhận được đồng thời.
- **Log Server**: Server hiển thị chi tiết các hoạt động (Connect, Login, Message) trên Terminal theo thời gian thực.
- **Xử lý đa luồng**: Hỗ trợ nhiều client kết nối cùng lúc mà không gây treo server.

## 🛠 Công nghệ sử dụng
- **Ngôn ngữ**: C# (.NET 6.0/7.0/8.0)
- **Giao tiếp mạng**: `System.Net.Sockets` (TCP Listener & TCP Client)
- **Định dạng dữ liệu**: `System.Text.Json` (Để đóng gói Packet)
- **Database**: SQLite (Thư viện `Microsoft.Data.Sqlite`)

## 📂 Cấu trúc dự án
- **ChatShared**: Thư viện dùng chung chứa class `Packet`, định nghĩa cấu trúc dữ liệu giữa Client và Server.
- **ChatServer**: Ứng dụng Console quản lý kết nối và điều phối tin nhắn.
- **ChatClient**: Ứng dụng Windows Forms (WinForms) cung cấp giao diện cho người dùng cuối.

## ⚙️ Hướng dẫn cài đặt và chạy
1. **Yêu cầu**: Cài đặt Visual Studio 2022 và .NET SDK mới nhất.
2. **Database**: Server sẽ tự động tạo file `chatapp.db` khi chạy lần đầu tiên.
3. **Thứ tự chạy**:
   - Chạy dự án `ChatServer` trước để mở cổng lắng nghe (Port 9999).
   - Chạy một hoặc nhiều dự án `ChatClient` để bắt đầu chat.
4. **Cấu hình IP**: Mặc định kết nối tới `127.0.0.1`.

## 📌 Trạng thái Project
- [x] Giao diện đăng nhập/chat
- [x] Quản lý Database người dùng
- [x] Chat Broadcast ổn định
- [ ] Tính năng gửi File (Đang phát triển)