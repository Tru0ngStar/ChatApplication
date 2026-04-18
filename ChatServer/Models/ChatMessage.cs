namespace ChatServer.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
