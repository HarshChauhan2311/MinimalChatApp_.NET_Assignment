using System.ComponentModel.DataAnnotations;

namespace MinimalChatApp.DTO
{
    public class DeleteGroupRequestDTO
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
