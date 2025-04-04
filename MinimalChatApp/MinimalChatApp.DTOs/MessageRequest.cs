using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTOs
{
    public class MessageRequest
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
