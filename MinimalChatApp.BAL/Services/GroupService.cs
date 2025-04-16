
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.Services
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

        public async Task<ServiceResponseDTO<GroupResponseDTO>> CreateGroupAsync(string groupName, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupName))
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Group name already exists.", StatusCode = 409 };

                var exists = await _groupRepo.GroupNameExistsAsync(groupName);
                if (exists)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Group name already exists.", StatusCode = 409 };

                var group = await _groupRepo.CreateGroupAsync(groupName, userId);
                if (group == null)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Failed to create group.", StatusCode = 500 };

                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = true, Data = group, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Internal error occurred.", StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<GroupResponseDTO>> UpdateGroupNameAsync(int groupId, string newName)
        {
            try
            {
                if (groupId <= 0 || string.IsNullOrWhiteSpace(newName))
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Invalid group ID or name.", StatusCode = 400 };

                var group = await _groupRepo.GetByIdAsync(groupId);
                if (group == null)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Group not found.", StatusCode = 404 };

                var exists = await _groupRepo.GroupNameExistsAsync(newName, groupId);
                if (exists)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Group name already exists.", StatusCode = 409 };

                var updated = await _groupRepo.UpdateGroupNameAsync(group, newName);
                if (updated == null)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Failed to update group name.", StatusCode = 500 };

                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = true, Data = updated, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Internal error occurred.", StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<GroupResponseDTO>> DeleteGroupAsync(int groupId, string name, int userId, int currentUserId)
        {
            try
            {
                var groupEntity = await _groupRepo.GetByIdAndNameAsync(groupId, name);
                if (groupEntity == null)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Group not found or already deleted.", StatusCode = 404 };

                if (currentUserId != userId || groupEntity.CreatedBy != userId)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Only the group creator can delete the group.", StatusCode = 403 };

                var deleted = await _groupRepo.DeleteGroupAsync(groupEntity);

                if (deleted != null)
                    return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Failed to delete the group.", StatusCode = 500 };

                var dto = new GroupResponseDTO
                {
                    Id = groupEntity.GroupId,
                    Name = groupEntity.GroupName,
                };

                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = true, Data = dto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<GroupResponseDTO> { IsSuccess = false, Error = "Internal error occurred.", StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<List<GroupResponseDTO>>> GetAllGroupsAsync(int userId)
        {
            try
            {
                var groups = await _groupRepo.GetAllGroupsWithMembersAsync(userId);
                var groupResponseList = groups.Select(g =>
                {
                    // Directly use the Members property as a List<string>
                    var memberEmails = g.Members?.ToList() ?? new List<string>();
                    var creatorEmail = g.CreatedBy;
                    if (!string.IsNullOrEmpty(creatorEmail) && !memberEmails.Contains(creatorEmail))
                        memberEmails.Add(creatorEmail);

                    return new GroupResponseDTO
                    {
                        Id = g.Id,
                        Name = g.Name,
                        CreatedAt = g.CreatedAt,
                        CreatedBy = creatorEmail ?? "unknown",
                        Members = memberEmails.Distinct().ToList()
                    };
                }).ToList();

                return new ServiceResponseDTO<List<GroupResponseDTO>>
                {
                    IsSuccess = true,
                    Data = groupResponseList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<List<GroupResponseDTO>>
                {
                    IsSuccess = false,
                    Error = "Internal error occurred.",
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponseDTO<GroupResponseDTO?>> GetGroupByIdAsync(int groupId)
        {
            try
            {
                var group = await _groupRepo.GetGroupByIdAsync(groupId);
                if (group == null)
                    return new ServiceResponseDTO<GroupResponseDTO?>
                    {
                        IsSuccess = false,
                        Error = "Group not found.",
                        StatusCode = 404
                    };

                var groupResponse = new GroupResponseDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    CreatedAt = group.CreatedAt,
                    CreatedBy = group.CreatedBy,
                    Members = group.Members?.Distinct().ToList() ?? new List<string>() // Ensure unique members
                };

                return new ServiceResponseDTO<GroupResponseDTO?>
                {
                    IsSuccess = true,
                    Data = groupResponse,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<GroupResponseDTO?>
                {
                    IsSuccess = false,
                    Error = "Internal error occurred.",
                    StatusCode = 500
                };
            }
        }

    }
}
