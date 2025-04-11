namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class UpdateGroupRequest
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
