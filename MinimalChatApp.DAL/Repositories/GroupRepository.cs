using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Group?> GetByNameAsync(string name)
        {

            return await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == name);

        }

        public async Task<Group?> GetByIdAsync(int groupId)
        {

            return await _context.Groups.FindAsync(groupId);

        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {

            return await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

        }

        public async Task<Group?> GetByIdAndNameAsync(int groupId, string name)
        {

            return await _context.Groups
                .Include(g => g.Creator)
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.GroupName == name);

        }

        public async Task<bool> GroupNameExistsAsync(string name, int? excludeGroupId = null)
        {

            return await _context.Groups.AnyAsync(g => g.GroupName == name && (!excludeGroupId.HasValue || g.GroupId != excludeGroupId));

        }

        public async Task<Group?> CreateGroupAsync(string groupName, int userId)
        {

            var group = new Group
            {
                GroupName = groupName,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Groups.AddAsync(group);
            var saved = await _context.SaveChangesAsync() > 0;
            return saved ? group : null;

        }

        public async Task<Group?> UpdateGroupNameAsync(Group group, string newName)
        {

            group.GroupName = newName;
            return await _context.SaveChangesAsync() > 0 ? group : null;

        }

        public async Task<bool> DeleteGroupAsync(Group group)
        {

            _context.Groups.Remove(group);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> IsAlreadyMemberAsync(int userId, int groupId)
        {

            return await _context.GroupMembers
                .AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);

        }

        public async Task<GroupMember?> AddMemberAsync(int userId, int groupId)
        {

            var member = new GroupMember
            {
                UserId = userId,
                GroupId = groupId
            };

            await _context.GroupMembers.AddAsync(member);
            var saved = await _context.SaveChangesAsync() > 0;

            return saved ? member : null;

        }

        public async Task<GroupMember?> GetByGroupMemberIdAsync(int groupMemberId)
        {

            return await _context.GroupMembers.FindAsync(groupMemberId);

        }

        public async Task<GroupMember?> UpdateAccessAsync(GroupMember member, GroupAccessType accessType, int? days)
        {

            member.AccessType = accessType;
            member.Days = accessType == GroupAccessType.Days ? days : null;

            var saved = await _context.SaveChangesAsync() > 0;
            return saved ? member : null;

        }

        public async Task<bool> RemoveMemberAsync(GroupMember member)
        {

            _context.GroupMembers.Remove(member);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> GroupExistsAsync(int groupId)
        {

            return await _context.Groups.AnyAsync(g => g.GroupId == groupId);

        }

        public async Task<bool> IsUserGroupMemberAsync(int userId, int groupId)
        {

            return await _context.Groups
                .AnyAsync(gm => gm.GroupId == groupId && gm.CreatedBy == userId);
        }

        public async Task<List<Group>> GetAllGroupsWithMembersAsync(int userId)
        {
            return await _context.Groups
                 .Where(g => g.Members.Any(m => m.UserId == userId))
                 .Include(g => g.Creator)
                 .Include(g => g.Members)
                     .ThenInclude(m => m.User)
                 .ToListAsync();

        }
    }
}
