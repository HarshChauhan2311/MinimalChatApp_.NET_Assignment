using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
