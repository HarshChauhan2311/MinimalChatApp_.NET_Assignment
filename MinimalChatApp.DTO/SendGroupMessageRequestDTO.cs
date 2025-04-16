using Microsoft.AspNetCore.Http;

namespace MinimalChatApp.DTO
{
    public class SendGroupMessageRequestDTO
    {
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
        public IFormFile? Attachment { get; set; }
    }
}
