using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.ChatApp.Shared.DTOs
{
    public class MessageRequest
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
