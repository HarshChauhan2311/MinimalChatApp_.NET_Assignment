using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinimalChatApp.DTO
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }

        [JsonIgnore] // This will prevent ReceiverId from being serialized
        public int? GroupId { get; set; }
        public string Content { get; set; } = string.Empty;        
        public DateTime Timestamp { get; set; }

        [JsonIgnore] // This will prevent ReceiverId from being serialized
        public string? FileUrl { get; set; }

        [JsonIgnore] // This will prevent ReceiverId from being serialized
        public string? ContentType { get; set; }
    }
}
