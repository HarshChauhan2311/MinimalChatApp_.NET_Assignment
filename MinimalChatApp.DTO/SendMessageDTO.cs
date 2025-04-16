using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class SendMessageDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Optional: for logging/sorting



    }
}
