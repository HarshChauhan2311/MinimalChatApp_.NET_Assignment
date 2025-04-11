using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;

        public GroupRepository(AppDbContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }

        public async Task<Group?> GetByNameAsync(string name)
        {
            try
            {
                return await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == name);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<Group?> GetByIdAsync(int groupId)
        {
            try
            {
                return await _context.Groups.FindAsync(groupId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            try
            {
                return await _context.Groups
                    .Include(g => g.Creator)
                    .Include(g => g.Members)
                        .ThenInclude(m => m.User)
                    .FirstOrDefaultAsync(g => g.GroupId == groupId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<Group?> GetByIdAndNameAsync(int groupId, string name)
        {
            try
            {
                return await _context.Groups
                    .Include(g => g.Creator)
                    .FirstOrDefaultAsync(g => g.GroupId == groupId && g.GroupName == name);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<bool> GroupNameExistsAsync(string name, int? excludeGroupId = null)
        {
            try
            {
                return await _context.Groups.AnyAsync(g => g.GroupName == name && (!excludeGroupId.HasValue || g.GroupId != excludeGroupId));
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<Group?> CreateGroupAsync(string groupName, int userId)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<Group?> UpdateGroupNameAsync(Group group, string newName)
        {
            try
            {
                group.GroupName = newName;
                return await _context.SaveChangesAsync() > 0 ? group : null;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<bool> DeleteGroupAsync(Group group)
        {
            try
            {
                _context.Groups.Remove(group);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<bool> IsAlreadyMemberAsync(int userId, int groupId)
        {
            try
            {
                return await _context.GroupMembers
                    .AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<GroupMember?> AddMemberAsync(int userId, int groupId)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<GroupMember?> GetByGroupMemberIdAsync(int groupMemberId)
        {
            try
            {
                return await _context.GroupMembers.FindAsync(groupMemberId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<GroupMember?> UpdateAccessAsync(GroupMember member, GroupAccessType accessType, int? days)
        {
            try
            {
                member.AccessType = accessType;
                member.Days = accessType == GroupAccessType.Days ? days : null;

                var saved = await _context.SaveChangesAsync() > 0;
                return saved ? member : null;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<bool> RemoveMemberAsync(GroupMember member)
        {
            try
            {
                _context.GroupMembers.Remove(member);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<bool> GroupExistsAsync(int groupId)
        {
            try
            {
                return await _context.Groups.AnyAsync(g => g.GroupId == groupId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<bool> IsUserGroupMemberAsync(int userId, int groupId)
        {
            try
            {
                return await _context.Groups
                    .AnyAsync(gm => gm.GroupId == groupId && gm.CreatedBy == userId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false;
            }
        }

        public async Task<List<Group>> GetAllGroupsWithMembersAsync(int userId)
        {
            try
            {
                return await _context.Groups
                     .Where(g => g.Members.Any(m => m.UserId == userId))
                     .Include(g => g.Creator)
                     .Include(g => g.Members)
                         .ThenInclude(m => m.User)
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new List<Group>();
            }
        }
    }
}
