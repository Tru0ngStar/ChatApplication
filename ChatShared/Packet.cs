namespace ChatShared
{
    public enum PacketType
    {
        Register,
        Login,
        Message,
        File
    }

    public class Packet
    {
        public PacketType Type { get; set; }
        public string? Sender { get; set; }
        public string? Password { get; set; }
        public string? Content { get; set; }
        public bool IsSuccess { get; set; }

        // Các trường phục vụ gửi File (Dùng cho bước tiếp theo)
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public byte[]? FileData { get; set; }

        public Packet(PacketType type)
        {
            Type = type;
        }
    }
}