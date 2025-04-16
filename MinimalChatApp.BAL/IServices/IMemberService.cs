using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Service for group member management.
    /// </summary>
    public interface IMemberService
    {
        /// <summary>
        /// Adds a user to the specified group as a member.
        /// </summary>
        /// <param name="request">Contains the user ID and group ID for the member to be added.</param>
        /// <returns>A ServiceResponseDTO indicating success, an optional error message, and the added group member.</returns>
        Task<ServiceResponseDTO<AddMemberResponseDTO?>> AddMemberAsync(AddMemberRequestDTO request);

        /// <summary>
        /// Removes a member from the group.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member to remove.</param>
        /// <returns>A ServiceResponseDTO indicating success, an optional error message, and a status code.</returns>
        Task<ServiceResponseDTO<string?>> RemoveMemberAsync(int groupMemberId);

    }
}
