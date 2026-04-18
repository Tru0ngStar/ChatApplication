namespace ChatShared
{
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
        LeaveGroup,
        FileList,
        FileListRequest,
        FileDownloadRequest,
        FileDownloadResponse
    }

    public class Packet
    {
        public PacketType Type { get; set; }
        public string? Sender { get; set; }
        public string? Password { get; set; }
        public string? Content { get; set; }
        public bool IsSuccess { get; set; }

        // Trường cho file transfer
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; }
        public int FileId { get; set; }
        public List<FileInfo>? FileList { get; set; }

        // Trường cho group chat
        public string? GroupName { get; set; }
        public int GroupId { get; set; }  // NEW: ID nhóm thay vì dùng tên
        public List<string>? UserList { get; set; }

        public Packet(PacketType type)
        {
            Type = type;
            UserList = new List<string>();
            GroupId = -1; // Mặc định: không phải nhóm nào
        }
    }

    public class FileInfo
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string? Sender { get; set; }
        public DateTime UploadedTime { get; set; }
    }
}