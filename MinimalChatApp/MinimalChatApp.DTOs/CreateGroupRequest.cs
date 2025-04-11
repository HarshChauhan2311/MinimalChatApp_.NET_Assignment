using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class CreateGroupRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
