using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DTO
{
    public class AddMemberResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int GroupId { get; set; } 
    }
}
