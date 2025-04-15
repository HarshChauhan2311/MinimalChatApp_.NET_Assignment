using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class CreateGroupRequestDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
