using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class AddMemberRequestDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GroupId { get; set; }
    }
}
