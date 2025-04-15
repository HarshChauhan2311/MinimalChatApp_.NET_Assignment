using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.Entity
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment PK
        public int GroupId { get; set; } // Auto-increment primary key

        [Required]
        [MaxLength(100)]
        public required string GroupName { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public ApplicationUser Creator { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    }
}
