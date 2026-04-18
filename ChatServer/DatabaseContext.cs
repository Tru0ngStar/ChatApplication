using ChatServer.Data;
using ChatServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatServer
{
    public static class DatabaseContext
    {
        private static ChatDbContext? _dbContext;

        public static void Initialize(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
            try
            {
                // Tạo database nếu chưa tồn tại
                _dbContext.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khởi tạo database: {ex.Message}");
            }
        }

        public static bool RegisterUser(string user, string pass)
        {
            try
            {
                if (_dbContext == null) return false;

                // Kiểm tra user đã tồn tại
                if (_dbContext.Users.Any(u => u.Username == user))
                    return false;

                var newUser = new User
                {
                    Username = user,
                    Password = pass,
                    CreatedAt = DateTime.Now
                };

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi đăng ký user: {ex.Message}");
                return false;
            }
        }

        public static bool ValidateUser(string user, string pass)
        {
            try
            {
                if (_dbContext == null) return false;

                var foundUser = _dbContext.Users.FirstOrDefault(u => u.Username == user && u.Password == pass);

                if (foundUser != null)
                {
                    foundUser.LastLogin = DateTime.Now;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xác thực user: {ex.Message}");
                return false;
            }
        }

        // Lưu chat message (DISABLED - chỉ hiển thị trực tiếp)
        // public static void SaveChatMessage(string sender, string content, int groupId) { }


        public static void SaveFileUpload(string fileName, long fileSize, string uploadedBy, string filePath, int groupId)
        {
            try
            {
                if (_dbContext == null) return;

                var fileRecord = new FileUpload
                {
                    FileName = fileName,
                    FileSize = fileSize,
                    UploadedBy = uploadedBy,
                    FilePath = filePath,
                    GroupId = groupId,
                    UploadedAt = DateTime.Now
                };

                _dbContext.FileUploads.Add(fileRecord);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi lưu file upload: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"   Chi tiết: {ex.InnerException.Message}");
            }
        }

        // Lấy danh sách file của group
        public static List<FileUpload> GetFilesByGroup(int groupId)
        {
            try
            {
                if (_dbContext == null) return new List<FileUpload>();

                return _dbContext.FileUploads
                    .Where(f => f.GroupId == groupId)
                    .OrderByDescending(f => f.UploadedAt)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi lấy danh sách file: {ex.Message}");
                return new List<FileUpload>();
            }
        }
    }
}
