using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MinimalChatApp.Entity
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

        public string? FileUrl { get; set; }
        public string? ContentType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;



        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public ApplicationUser? Receiver { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}
