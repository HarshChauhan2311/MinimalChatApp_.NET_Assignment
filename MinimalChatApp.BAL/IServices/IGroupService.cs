using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Service for group and group member management.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Creates a new group with the specified name and creator user ID.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="userId">The ID of the user creating the group.</param>
        /// <returns>A tuple indicating success, error message, and the created group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> CreateGroupAsync(string groupName, int userId);

        /// <summary>
        /// Updates the name of an existing group.
        /// </summary>
        /// <param name="groupId">The ID of the group to update.</param>
        /// <param name="newName">The new group name.</param>
        /// <returns>A tuple indicating success, error message, and the updated group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> UpdateGroupNameAsync(int groupId, string newName);

        /// <summary>
        /// Deletes a group by ID and name with user validation.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="name">The name of the group.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="currentUserId">The ID of the authenticated user.</param>
        /// <returns>A tuple indicating success, error message, and the deleted group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> DeleteGroupAsync(int groupId, string name, int userId, int currentUserId);

        /// <summary>
        /// Adds a user as a member to a group.
        /// </summary>
        /// <param name="userId">The ID of the user to add.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>A tuple indicating success, error message, and the added group member.</returns>
        Task<(bool isSuccess, string? error, GroupMember? member)> AddMemberAsync(int userId, int groupId);

        /// <summary>
        /// Updates access permissions for a group member.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member.</param>
        /// <param name="accessType">The type of access to assign.</param>
        /// <param name="days">Optional number of days the access is valid.</param>
        /// <returns>A tuple indicating success, error message, and the updated group member.</returns>
        Task<(bool isSuccess, string? error, GroupMember? member)> UpdateMemberAccessAsync(int groupMemberId, GroupAccessType accessType, int? days);

        /// <summary>
        /// Removes a member from a group.
        /// </summary>
        /// <param name="groupMemberId">The ID of the group member to remove.</param>
        /// <returns>A tuple indicating success and error message if any.</returns>
        Task<(bool isSuccess, string? error)> RemoveMemberAsync(int groupMemberId);

        /// <summary>
        /// Retrieves all groups along with their members for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of group DTOs.</returns>
        Task<List<GroupResponseDTO>> GetAllGroupsAsync(int userId);

        /// <summary>
        /// Retrieves group details by group ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>The group DTO if found; otherwise, null.</returns>
        Task<GroupResponseDTO?> GetGroupByIdAsync(int groupId);
    }

}
