using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Repositories
{
    public class MemberRepository : GenericRepository<GroupMember>,IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context) : base(context)
        {
            _context = context;
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
    }
}
