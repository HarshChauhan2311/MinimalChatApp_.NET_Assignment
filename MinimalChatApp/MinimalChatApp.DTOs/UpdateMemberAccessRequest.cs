using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.DTOs
{
    public class UpdateMemberAccessRequest
    {
        public int GroupMemberId { get; set; }
        public GroupAccessType AccessType { get; set; }
        public int? Days { get; set; }
    }
}
