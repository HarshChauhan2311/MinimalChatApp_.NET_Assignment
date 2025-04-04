using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalChatApp.Models
{
    public class RequestLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment PK
        public int Id { get; set; }

        public string? IPAddress { get; set; }
        public string? Username { get; set; }
        public string? RequestBody { get; set; }
        public string? RequestPath { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
