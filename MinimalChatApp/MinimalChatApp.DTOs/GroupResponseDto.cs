namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class GroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
