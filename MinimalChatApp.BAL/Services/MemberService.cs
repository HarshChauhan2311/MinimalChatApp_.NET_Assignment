using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.DTO;
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

        public async Task<ServiceResponseDTO<AddMemberResponseDTO?>> AddMemberAsync(AddMemberRequestDTO request)
        {
            try
            {
                if (request.UserId <= 0 || request.GroupId <= 0)
                    return new ServiceResponseDTO<AddMemberResponseDTO?>
                    {
                        IsSuccess = false,
                        Error = "Invalid userId or groupId.",
                        StatusCode = 400
                    };

                var exists = await _memberRepo.IsAlreadyMemberAsync(request.UserId, request.GroupId);
                if (exists)
                    return new ServiceResponseDTO<AddMemberResponseDTO?>
                    {
                        IsSuccess = false,
                        Error = "User is already a member of the group.",
                        StatusCode = 409 // Conflict
                    };

                var existsGroup = await _groupRepo.GetGroupByIdAsync(request.GroupId);
                if (existsGroup == null)
                    return new ServiceResponseDTO<AddMemberResponseDTO?>
                    {
                        IsSuccess = false,
                        Error = "Group with ID: " + request.GroupId + " does not exist.",
                        StatusCode = 404
                    };

                var added = await _memberRepo.AddMemberAsync(request);
                if (added == null)
                    return new ServiceResponseDTO<AddMemberResponseDTO?>
                    {
                        IsSuccess = false,
                        Error = "Failed to add member to group.",
                        StatusCode = 500
                    };

                return new ServiceResponseDTO<AddMemberResponseDTO?>
                {
                    IsSuccess = true,
                    Data = added,
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<AddMemberResponseDTO?>
                {
                    IsSuccess = false,
                    Error = "Internal error occurred.",
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponseDTO<string?>> RemoveMemberAsync(int groupMemberId)
        {
            try
            {
                // Validate the group member ID
                if (groupMemberId <= 0)
                {
                    return new ServiceResponseDTO<string?>
                    {
                        IsSuccess = false,
                        Error = "Invalid group member ID.",
                        StatusCode = 400
                    };
                }

                // Fetch the member from the repository
                var member = await _memberRepo.GetByGroupMemberIdAsync(groupMemberId);
                if (member == null)
                {
                    return new ServiceResponseDTO<string?>
                    {
                        IsSuccess = false,
                        Error = "Group member not found.",
                        StatusCode = 404
                    };
                }

                await _memberRepo.RemoveMemberAsync(member);

                return new ServiceResponseDTO<string?>
                {
                    IsSuccess = true,
                    Data = "Member deleted successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<string?>
                {
                    IsSuccess = false,
                    Error = "Internal server error: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

    }
}
