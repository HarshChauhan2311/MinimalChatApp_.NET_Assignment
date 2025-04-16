using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinimalChatApp.DTO
{
    public class ConversationMessageDTO : MessageDTO
    {
        [JsonIgnore] // This will prevent ReceiverId from being serialized
        public int RecipientId { get; set; }
    }
}
