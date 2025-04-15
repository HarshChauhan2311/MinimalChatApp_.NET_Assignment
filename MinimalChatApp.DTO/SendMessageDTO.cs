using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class SendMessageDto
    {
        public string FromUser { get; set; }     // Sender's username
        public string ToUser { get; set; }       // Receiver's username
        public string Message { get; set; }      // Actual message content
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Optional: for logging/sorting
    }
}
