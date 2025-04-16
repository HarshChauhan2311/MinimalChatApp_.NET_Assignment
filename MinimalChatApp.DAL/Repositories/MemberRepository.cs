using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.DTO;
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

        public async Task<AddMemberResponseDTO> AddMemberAsync(AddMemberRequestDTO request)
        {
            try
            {
                // Validate request data (e.g., check if UserId and GroupId are valid)
                if (request.UserId <= 0 || request.GroupId <= 0)
                {
                    return new AddMemberResponseDTO
                    {
                        Message = "Invalid user or group ID."
                    };
                }

                // Create the new member object
                var member = new GroupMember
                {
                    UserId = request.UserId,
                    GroupId = request.GroupId,
                    AccessType = request.AccessType,
                    Days = request.AccessType == GroupAccessType.Days ? request.Days : null
                };

                // Add the new member to the GroupMembers table
                await _context.GroupMembers.AddAsync(member);
                var saved = await _context.SaveChangesAsync() > 0;

                // Return a well-structured response
                if (saved)
                {
                    return new AddMemberResponseDTO
                    {
                        Id = member.Id,
                        UserId = member.UserId,
                        GroupId = member.GroupId,
                        Message = "Member added successfully."
                    };
                }
                else
                {
                    return new AddMemberResponseDTO
                    {
                        Message = "Failed to add member."
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you may want to add a logging mechanism here)
                return new AddMemberResponseDTO
                {
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }


        public async Task<GroupMember?> GetByGroupMemberIdAsync(int groupMemberId)
        {

            return await _context.GroupMembers.FindAsync(groupMemberId);

        }

        public async Task<RemoveMemberResponseDTO> RemoveMemberAsync(GroupMember member)
        {
            var response = new RemoveMemberResponseDTO();

            // Try to remove the member from the group
            _context.GroupMembers.Remove(member);
            var result = await _context.SaveChangesAsync();

            // Check if the deletion was successful
            if (result > 0)
            {
                response.Message = "Member deleted successfully.";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = "Failed to delete member.";
                response.IsSuccess = false;
            }

            return response;
        }


    }
}
