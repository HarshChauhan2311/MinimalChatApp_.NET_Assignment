using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class DeleteGroupRequest
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
