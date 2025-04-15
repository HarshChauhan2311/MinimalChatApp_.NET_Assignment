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
        /// Creates a new group with the specified name and creator's user ID.
        /// </summary>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="userId">The ID of the user creating the group.</param>
        /// <returns>A tuple indicating success, an optional error message, and the created group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> CreateGroupAsync(string groupName, int userId);

        /// <summary>
        /// Updates the name of an existing group.
        /// </summary>
        /// <param name="groupId">The ID of the group to update.</param>
        /// <param name="newName">The new name for the group.</param>
        /// <returns>A tuple indicating success, an optional error message, and the updated group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> UpdateGroupNameAsync(int groupId, string newName);

        /// <summary>
        /// Deletes a group by ID and name, validating the user performing the action.
        /// </summary>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <param name="name">The name of the group to confirm deletion.</param>
        /// <param name="userId">The ID of the group creator or authorized user.</param>
        /// <param name="currentUserId">The ID of the currently authenticated user.</param>
        /// <returns>A tuple indicating success, an optional error message, and the deleted group.</returns>
        Task<(bool isSuccess, string? error, Group? group)> DeleteGroupAsync(int groupId, string name, int userId, int currentUserId);

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

        /// <summary>
        /// Retrieves all groups and their members for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting group information.</param>
        /// <returns>A list of group details.</returns>
        Task<List<GroupResponseDTO>> GetAllGroupsAsync(int userId);

        /// <summary>
        /// Retrieves details of a specific group by its ID.
        /// </summary>
        /// <param name="groupId">The ID of the group to retrieve.</param>
        /// <returns>The group details if found; otherwise, null.</returns>
        Task<GroupResponseDTO?> GetGroupByIdAsync(int groupId);
    }


}
