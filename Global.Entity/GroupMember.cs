using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalChatApp.Entity
{
    public class GroupMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment PK
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        public GroupAccessType AccessType { get; set; } = GroupAccessType.All;

        public int? Days { get; set; }

        // Navigation
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
