using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly AppDbContext _context;
        private readonly IGenericRepository<Group> _genericGroupRepo;

        public GroupRepository(AppDbContext context, IGenericRepository<Group> genericGroupRepo) : base(context)
        {
            _context = context;
            _genericGroupRepo = genericGroupRepo;
        }

        public async Task<GroupResponseDTO?> GetByNameAsync(string name)
        {
            var group = await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupName == name);

            if (group == null) return null;

            return new GroupResponseDTO
            {
                Id = group.GroupId,
                Name = group.GroupName,
                CreatedAt = group.CreatedAt,
                CreatedBy = group.Creator.Email,
                Members = group.Members.Select(m => m.User.Email).ToList()
            };
        }


        public async Task<Group?> GetByIdAsync(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null) return null;

            return new Group
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                CreatedAt = group.CreatedAt,
                CreatedBy = group.CreatedBy
            };
        }


        public async Task<GroupResponseDTO?> GetGroupByIdAsync(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null) return null;

            return new GroupResponseDTO
            {
                Id = group.GroupId,
                Name = group.GroupName,
                CreatedAt = group.CreatedAt,
                CreatorId = group.CreatedBy
            };
        }


        public async Task<Group?> GetByIdAndNameAsync(int groupId, string name)
        {
            var group = await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.GroupName == name);

            if (group == null)
                return null;

            return await _context.Groups
            .Include(g => g.Creator)
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.GroupId == groupId && g.GroupName == name);
        }


        public async Task<bool> GroupNameExistsAsync(string name, int? excludeGroupId = null)
        {

            name = name.Trim().ToLower();

            return await _context.Groups
                .AnyAsync(g =>
                    g.GroupName.ToLower().Trim() == name &&
                    (!excludeGroupId.HasValue || g.GroupId != excludeGroupId));

        }

        public async Task<GroupResponseDTO?> CreateGroupAsync(string groupName, int userId)
        {

            var group = new Group
            {
                GroupName = groupName,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _genericGroupRepo.AddAsync(group);
            var saved = await _genericGroupRepo.SaveChangesAsync();
            if (!saved) return null;

            // Get creator info
            var creator = await _context.Users.FindAsync(userId);
            if (creator == null) return null;

            // Map to DTO
            return new GroupResponseDTO
            {
                Id = group.GroupId,
                Name = group.GroupName,
                CreatedAt = group.CreatedAt,
                CreatedBy = creator.Email,
                Members = new List<string>() // New group, no members yet
            };

        }

        public async Task<GroupResponseDTO?> UpdateGroupNameAsync(Group group, string newName)
        {
            var existingGroup = await _context.Groups.AsNoTracking()
       .Include(g => g.Members) // Assuming you have navigation property for Members
       .FirstOrDefaultAsync(g => g.GroupId == group.GroupId);

            if (existingGroup == null)
                return null;

            existingGroup.GroupName = newName;

            _genericGroupRepo.Update(existingGroup);
            var saved = await _genericGroupRepo.SaveChangesAsync();

            if (!saved)
                return null;

            return new GroupResponseDTO
            {
                Id = existingGroup.GroupId,
                Name = existingGroup.GroupName,
            };

        }

        public async Task<GroupResponseDTO> DeleteGroupAsync(Group group)
        {
            var existingGroup = await _context.Groups.AsNoTracking()
                    .Include(g => g.Members) // if applicable
                    .FirstOrDefaultAsync(g => g.GroupId == group.GroupId);

            if (existingGroup == null)
                return null;

            _genericGroupRepo.Delete(existingGroup);
            var deleted = await _genericGroupRepo.SaveChangesAsync();

            if (!deleted)
                return null;

            return new GroupResponseDTO
            {
                Id = existingGroup.GroupId,
                Name = existingGroup.GroupName,
            };

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

        public async Task<List<GroupResponseDTO>> GetAllGroupsWithMembersAsync(int userId)
        {
            var groups = await _context.Groups
                 .Where(g => g.Members.Any(m => m.UserId == userId))
                 .Include(g => g.Creator)
                 .Include(g => g.Members)
                     .ThenInclude(m => m.User)
                 .ToListAsync();

            var groupDTOs = groups.Select(g => new GroupResponseDTO
            {
                Id = g.GroupId,
                Name = g.GroupName,
                CreatedAt = g.CreatedAt,
                CreatedBy = g.Creator.Email,
                Members = g.Members.Select(m => m.User.Email).ToList()
            }).ToList();

            return groupDTOs;

        }
    }
}
