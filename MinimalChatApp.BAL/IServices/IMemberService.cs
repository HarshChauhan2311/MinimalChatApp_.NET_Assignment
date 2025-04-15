using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="userId">The ID of the user to add to the group.</param>
        /// <param name="groupId">The ID of the target group.</param>
        /// <returns>A tuple indicating success, an optional error message, and the added group member.</returns>
        Task<(bool isSuccess, string? error, GroupMember? member)> AddMemberAsync(int userId, int groupId);

        /// <summary>
        /// Updates the access level and optional access duration for a group member.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member to update.</param>
        /// <param name="accessType">The new access level to assign.</param>
        /// <param name="days">Optional duration in days for the access validity.</param>
        /// <returns>A tuple indicating success, an optional error message, and the updated group member.</returns>
        Task<(bool isSuccess, string? error, GroupMember? member)> UpdateMemberAccessAsync(int groupMemberId, GroupAccessType accessType, int? days);

        /// <summary>
        /// Removes a member from the group.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member to remove.</param>
        /// <returns>A tuple indicating success and an optional error message.</returns>
        Task<(bool isSuccess, string? error)> RemoveMemberAsync(int groupMemberId);
    }
}
