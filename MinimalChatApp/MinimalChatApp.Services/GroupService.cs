using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.DTOs;
using MinimalChatApp.MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IErrorLogService _errorLogService;

        public GroupService(IGroupRepository groupRepo, IErrorLogService errorLogService)
        {
            _groupRepo = groupRepo;
            _errorLogService = errorLogService;
        }

        public async Task<(bool isSuccess, string? error, Group? group)> CreateGroupAsync(string groupName, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupName))
                    return (false, "Group name is required.", null);

                var exists = await _groupRepo.GroupNameExistsAsync(groupName);
                if (exists)
                    return (false, "Group name already exists.", null);

                var group = await _groupRepo.CreateGroupAsync(groupName, userId);
                return group == null
                    ? (false, "Failed to create group.", null)
                    : (true, null, group);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.", null);
            }
        }

        public async Task<(bool isSuccess, string? error, Group? group)> UpdateGroupNameAsync(int groupId, string newName)
        {
            try
            {
                if (groupId <= 0 || string.IsNullOrWhiteSpace(newName))
                    return (false, "Invalid group ID or name.", null);

                var group = await _groupRepo.GetByIdAsync(groupId);
                if (group == null)
                    return (false, "Group not found.", null);

                var exists = await _groupRepo.GroupNameExistsAsync(newName, groupId);
                if (exists)
                    return (false, "Group name already exists.", null);

                var updated = await _groupRepo.UpdateGroupNameAsync(group, newName);
                return updated == null
                    ? (false, "Failed to update group name.", null)
                    : (true, null, updated);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.", null);
            }
        }

        public async Task<(bool isSuccess, string? error, Group? group)> DeleteGroupAsync(int groupId, string name, int userId, int currentUserId)
        {
            try
            {
                var group = await _groupRepo.GetByIdAndNameAsync(groupId, name);
                if (group == null)
                    return (false, "Group not found or already deleted.", null);

                if (group.CreatedBy != userId || currentUserId != userId)
                    return (false, "Only the group creator can delete the group.", null);

                var deleted = await _groupRepo.DeleteGroupAsync(group);
                return deleted
                    ? (true, null, group)
                    : (false, "Failed to delete the group.", null);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, "Internal error occurred.", null);
            }
        }

        public async Task<(bool isSuccess, string? error, GroupMember? member)> AddMemberAsync(int userId, int groupId)
        {
            try
            {
                if (userId <= 0 || groupId <= 0)
                    return (false, "Invalid userId or groupId.", null);

                var exists = await _groupRepo.IsAlreadyMemberAsync(userId, groupId);
                if (exists)
                    return (false, "User is already a member of the group.", null);

                var added = await _groupRepo.AddMemberAsync(userId, groupId);
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

                var member = await _groupRepo.GetByGroupMemberIdAsync(groupMemberId);
                if (member == null)
                    return (false, "Group member not found.", null);

                var updated = await _groupRepo.UpdateAccessAsync(member, accessType, days);
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

                var member = await _groupRepo.GetByGroupMemberIdAsync(groupMemberId);
                if (member == null)
                    return (false, "Group member not found.");

                var removed = await _groupRepo.RemoveMemberAsync(member);
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

        public async Task<List<GroupResponseDto>> GetAllGroupsAsync(int userId)
        {
            try
            {
                var groups = await _groupRepo.GetAllGroupsWithMembersAsync(userId);
                return groups.Select(g =>
                {
                    var memberEmails = g.Members?.Select(m => m.User.Email).ToList() ?? new List<string>();
                    var creatorEmail = g.Creator?.Email;
                    if (!string.IsNullOrEmpty(creatorEmail) && !memberEmails.Contains(creatorEmail))
                        memberEmails.Add(creatorEmail);

                    return new GroupResponseDto
                    {
                        Id = g.GroupId,
                        Name = g.GroupName,
                        CreatedAt = g.CreatedAt,
                        CreatedBy = creatorEmail ?? "unknown",
                        Members = memberEmails.Distinct().ToList()
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new List<GroupResponseDto>();
            }
        }

        public async Task<GroupResponseDto?> GetGroupByIdAsync(int groupId)
        {
            try
            {
                var group = await _groupRepo.GetGroupByIdAsync(groupId);
                if (group == null) return null;

                return new GroupResponseDto
                {
                    Id = group.GroupId,
                    Name = group.GroupName,
                    CreatedAt = group.CreatedAt,
                    CreatedBy = group.Creator?.Email ?? "unknown",
                    Members = group.Members?
                        .Select(m => m.User.Email)
                        .Distinct()
                        .ToList() ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }
    }
}
