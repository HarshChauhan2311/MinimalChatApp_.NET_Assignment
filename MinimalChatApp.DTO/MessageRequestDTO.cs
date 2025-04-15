using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class MessageRequestDTO
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
