using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DTO
{
    public class SentMessageDTO : MessageDTO
    {
        public int? ReceiverId { get; set; }
    }
}
