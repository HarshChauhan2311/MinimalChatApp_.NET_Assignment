using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.DTO;
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
        Task<AddMemberResponseDTO> AddMemberAsync(AddMemberRequestDTO request);

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
        /// Removes a member from a group.
        /// </summary>
        /// <param name="member">The group member to remove.</param>
        /// <returns>True if the member was removed successfully; otherwise, false.</returns>
        Task<RemoveMemberResponseDTO> RemoveMemberAsync(GroupMember member);
    }
}
