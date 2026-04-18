namespace ChatServer.Models
{
    public class FileUpload
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public string FilePath { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
}
