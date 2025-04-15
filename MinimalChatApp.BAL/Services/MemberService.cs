using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IErrorLogService _errorLogService;

        public MemberService(IMemberRepository memberRepo, IErrorLogService errorLogService, IGroupRepository groupRepo)
        {
            _memberRepo = memberRepo;
            _errorLogService = errorLogService;
            _groupRepo = groupRepo;
        }

        public async Task<(bool isSuccess, string? error, GroupMember? member)> AddMemberAsync(int userId, int groupId)
        {
            try
            {
                if (userId <= 0 || groupId <= 0)
                    return (false, "Invalid userId or groupId.", null);

                var exists = await _memberRepo.IsAlreadyMemberAsync(userId, groupId);
                if (exists)
                    return (false, "User is already a member of the group.", null);

                var existsGroup = await _groupRepo.GetGroupByIdAsync(groupId);
                if (existsGroup == null)
                    return (false, "Group with ID: " + groupId + " does not exist.", null);

                var added = await _memberRepo.AddMemberAsync(userId, groupId);
                return added != null
                    ? (true, null, added)
                    : (false, "Failed to add member to group.", null);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.", null);
            }
        }

        public async Task<(bool isSuccess, string? error, GroupMember? member)> UpdateMemberAccessAsync(int groupMemberId, GroupAccessType accessType, int? days)
        {
            try
            {
                if (groupMemberId <= 0)
                    return (false, "Invalid groupMemberId.", null);

                if (accessType == GroupAccessType.Days && (!days.HasValue || days <= 0))
                    return (false, "Days must be a positive number when access type is Days.", null);

                var member = await _memberRepo.GetByGroupMemberIdAsync(groupMemberId);
                if (member == null)
                    return (false, "Group member not found.", null);

                var updated = await _memberRepo.UpdateAccessAsync(member, accessType, days);
                return updated == null
                    ? (false, "Failed to update access.", null)
                    : (true, null, updated);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.", null);
            }
        }

        public async Task<(bool isSuccess, string? error)> RemoveMemberAsync(int groupMemberId)
        {
            try
            {
                if (groupMemberId <= 0)
                    return (false, "Invalid group member ID.");

                var member = await _memberRepo.GetByGroupMemberIdAsync(groupMemberId);
                if (member == null)
                    return (false, "Group member not found.");

                var removed = await _memberRepo.RemoveMemberAsync(member);
                return removed
                    ? (true, null)
                    : (false, "Failed to remove group member.");
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.");
            }
        }
    }
}
