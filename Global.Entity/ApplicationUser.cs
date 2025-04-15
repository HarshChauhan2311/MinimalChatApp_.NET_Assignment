using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MinimalChatApp.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        // Add any custom properties here
        public string? FullName { get; set; }

        // Add any custom properties here
        public string? Email { get; set; }
    }
}
