using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class AddMemberRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GroupId { get; set; }
    }
}
