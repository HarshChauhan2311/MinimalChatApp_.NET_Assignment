namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class SendGroupMessageRequest
    {
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
