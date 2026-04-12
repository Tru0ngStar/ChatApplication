# 📋 Hướng Dẫn Kiểm Tra Các Tính Năng

## 🧪 Kiểm Tra từng tính năng

### ✅ 1. Kiểm Tra Đăng Nhập/Đăng Ký

**Bước 1:** Chạy ChatServer
```
Kỳ vọng: 
- Console hiển thị: "[DB] Cơ sở dữ liệu tài khoản đã sẵn sàng."
- File "accounts.txt" được tạo trong thư mục ChatServer\bin\Debug\net10.0\
```

**Bước 2:** Chạy ChatClient
- Nhập username: `user1`
- Nhập password: `pass123`
- Bấm "Đăng ký"

```
Kỳ vọng:
- MessageBox: "Thành công!"
- Server console: "[REG] Thử đăng ký: user1 → Thành công"
- accounts.txt có dòng: "user1:pass123"
```

**Bước 3:** Thử đăng ký lại
- Bấm "Đăng ký" lại với username `user1`

```
Kỳ vọng:
- MessageBox: "Tài khoản tồn tại!"
- Server: "[REG] Thử đăng ký: user1 → Thất bại"
```

**Bước 4:** Đăng nhập
- Bấm "Đăng nhập"

```
Kỳ vọng:
- Giao diện chat được mở (grpChat.Enabled = true)
- Danh sách user online xuất hiện (user1)
- Tin nhắn hệ thống: "--- Đăng nhập thành công! ---" (xanh lá)
- Server: "[AUTH] user1 đã đăng nhập thành công."
```

---

### ✅ 2. Kiểm Tra Danh Sách User Online

**Bước 1:** Mở 2 Client cùng lúc
- Client A: Đăng nhập với `user1`
- Client B: Đăng ký `user2` rồi đăng nhập

```
Kỳ vọng:
- Client A ListBox: [user1, user2]
- Client B ListBox: [user1, user2]
- Danh sách cập nhật tự động khi user mới vào
```

---

### ✅ 3. Kiểm Tra Chat Toàn Server

**Bước 1:** 
- Client A gửi tin nhắn: "Chào mọi người"

```
Kỳ vọng:
- Client A thấy: "Tôi: Chào mọi người" (màu xanh)
- Client B thấy: "user1: Chào mọi người" (màu đỏ)
- Server console: "[MSG] user1: Chào mọi người"
```

**Bước 2:**
- Client B gửi tin nhắn: "Xin chào!"

```
Kỳ vọng:
- Client B thấy: "Tôi: Xin chào!" (màu xanh)
- Client A thấy: "user2: Xin chào!" (màu đỏ)
```

---

### ✅ 4. Kiểm Tra Gửi File

**Bước 1:** Tạo một file test
- Tạo file `test.txt` trong Desktop với nội dung "Hello World"

**Bước 2:**
- Client A bấm "Gửi File"
- Chọn file `test.txt`

```
Kỳ vọng:
- Client A thấy: "📤 Gửi file: test.txt (11 B)"
- Client B thấy: "📁 user1 gửi file: test.txt (11 B)"
- Server: "[FILE] user1 gửi file: test.txt"
- File tự động lưu trong: C:\Users\[YourUsername]\Downloads\test.txt
```

**Bước 3:** Kiểm tra file nhận được
```
Kỳ vọng:
- Mở file trong Downloads
- Nội dung: "Hello World"
```

---

### ✅ 5. Kiểm Tra Group Chat

**Bước 1:**
- Client A nhập: `Project_A`
- Bấm "Tạo Nhóm"

```
Kỳ vọng:
- Client A thấy: "✅ Tạo nhóm: Project_A"
- Tin nhắn hệ thống: "HỆ THỐNG: user1 đã tạo nhóm: Project_A" (xanh)
- Server: "[GROUP] user1 tham gia nhóm: Project_A"
```

**Bước 2:**
- Client A gửi tin nhắn: "Chat nhóm Project_A"

```
Kỳ vọng:
- Client A thấy: "Tôi: Chat nhóm Project_A"
- Client B KHÔNG thấy tin nhắn này (vì chưa tham gia)
```

**Bước 3:**
- Client B nhập: `Project_A`
- Bấm "Tham gia Nhóm"

```
Kỳ vọng:
- Client B thấy: "✅ Tham gia nhóm: Project_A"
- Tin nhắn hệ thống: "HỆ THỐNG: user2 đã tham gia nhóm: Project_A"
```

**Bước 4:**
- Client B gửi tin nhắn: "Chào Project_A"

```
Kỳ vọng:
- Client A thấy: "user2: Chào Project_A" (màu đỏ)
- Client B thấy: "Tôi: Chào Project_A" (màu xanh)
```

**Bước 5:**
- Client A bấm "Rời Nhóm"

```
Kỳ vọng:
- Client A thấy: "✅ Rời khỏi nhóm: Project_A"
- Quay lại chat chung (không gửi tin nhắn nhóm)
```

---

### ✅ 6. Kiểm Tra Nhiều Client Đồng Thời (Multi-threading)

**Bước 1:** Mở 3 đến 5 Client cùng lúc
- Mỗi cái đăng nhập hoặc đăng ký khác nhau

```
Kỳ vọng:
- Tất cả đăng nhập thành công
- Server không bị treo
- Danh sách user online cập nhật trên tất cả client
- Mỗi client có thể gửi/nhận tin nhắn độc lập
```

**Bước 2:** Gửi tin nhắn từ tất cả client
```
Kỳ vọng:
- Tất cả tin nhắn được nhận trên tất cả client
- Không có delay hoặc mất tin nhắn
```

---

### ✅ 7. Kiểm Tra Danh Sách Tài Khoản

Sau tất cả các bước trên, mở file `accounts.txt`:

```
Kỳ vọng nội dung:
user1:pass123
user2:pass456
...
```

---

## 🐛 Troubleshooting

| Lỗi | Nguyên Nhân | Giải Pháp |
|-----|-----------|----------|
| Server không bật được | Port 9999 đang sử dụng | Đóng ứng dụng cũ hoặc đổi port |
| Client không kết nối | Server không chạy | Chắc chắn rằng ChatServer đang chạy |
| Không thấy user online | Chưa đăng nhập | Đăng nhập lại sau khi tạo tài khoản |
| File không lưu | Quyền folder Downloads | Kiểm tra quyền write của folder Downloads |
| Group message lỗi | Tên nhóm khác nhau | Chắc chắn tên nhóm giống nhau trên tất cả client |

---

## 📊 Checklist Hoàn Thành

- [ ] ✅ Đăng nhập/Đăng ký
- [ ] ✅ Danh sách user online
- [ ] ✅ Chat toàn server
- [ ] ✅ Gửi/nhận file
- [ ] ✅ Group chat
- [ ] ✅ Multi-threading (nhiều client)
- [ ] ✅ Tin nhắn có màu sắc
- [ ] ✅ File accounts.txt

---

**Nếu tất cả các checkbox ✅ đều được đánh, thì ứng dụng đã hoàn toàn hoạt động!** 🎉
