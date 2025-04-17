using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DTO
{
    public class GroupChatHistoryRequestDTO
    {

        public int GroupId { get; set; }
        public DateTime? Before { get; set; }
        public int Count { get; set; } = 20;
        public string Sort { get; set; } = "asc";
    }
}
