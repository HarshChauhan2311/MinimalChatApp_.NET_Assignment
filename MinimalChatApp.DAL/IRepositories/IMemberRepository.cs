using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.IRepositories
{
    /// <summary>
    /// Defines methods for managing group members in the repository.
    /// </summary>
    public interface IMemberRepository
    {
        /// <summary>
        /// Adds a user as a member to a group.
        /// </summary>
        /// <param name="userId">The ID of the user to add.</param>
        /// <param name="groupId">The ID of the group to add the user to.</param>
        /// <returns>The added group member entity.</returns>
        Task<GroupMember?> AddMemberAsync(int userId, int groupId);

        /// <summary>
        /// Checks if a user is already a member of a group.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>True if the user is already a member; otherwise, false.</returns>
        Task<bool> IsAlreadyMemberAsync(int userId, int groupId);

        /// <summary>
        /// Retrieves a group member by their group member ID.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member.</param>
        /// <returns>The group member entity if found; otherwise, null.</returns>
        Task<GroupMember?> GetByGroupMemberIdAsync(int groupMemberId);

        /// <summary>
        /// Updates the access type and optionally the duration of access for a group member.
        /// </summary>
        /// <param name="member">The group member to update.</param>
        /// <param name="accessType">The new access type.</param>
        /// <param name="days">Optional number of days for temporary access.</param>
        /// <returns>The updated group member entity.</returns>
        Task<GroupMember?> UpdateAccessAsync(GroupMember member, GroupAccessType accessType, int? days);

        /// <summary>
        /// Removes a member from a group.
        /// </summary>
        /// <param name="member">The group member to remove.</param>
        /// <returns>True if the member was removed successfully; otherwise, false.</returns>
        Task<bool> RemoveMemberAsync(GroupMember member);
    }
}
