namespace MinimalChatApp.DTO
{
    public class SendGroupMessageRequestDTO
    {
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
