using System.ComponentModel.DataAnnotations;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DTO
{
    public class AddMemberRequestDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GroupId { get; set; }

        // New fields
        public GroupAccessType AccessType { get; set; }  // 0 = none, 1 = all, 2 = limited
        public int? Days { get; set; }       // Optional when AccessType = 2
    }
}
