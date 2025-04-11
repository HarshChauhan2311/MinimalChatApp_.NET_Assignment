using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MinimalChatApp.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment PK
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        public int? ReceiverId { get; set; } 

        public int? GroupId { get; set; } // for group messages

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public User? Receiver { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}
