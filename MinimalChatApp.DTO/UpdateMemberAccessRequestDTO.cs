
using MinimalChatApp.Entity;

namespace MinimalChatApp.DTO
{
    public class UpdateMemberAccessRequestDTO
    {
        public int GroupMemberId { get; set; }
        public GroupAccessType AccessType { get; set; }
        public int? Days { get; set; }
    }
}
