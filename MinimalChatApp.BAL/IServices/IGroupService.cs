using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Service for group management.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Creates a new group with the specified name and creator's user ID.
        /// </summary>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="userId">The ID of the user creating the group.</param>
        /// <returns>A standard service response containing the created group.</returns>
        Task<ServiceResponseDTO<GroupResponseDTO>> CreateGroupAsync(string groupName, int userId);

        /// <summary>
        /// Updates the name of an existing group.
        /// </summary>
        /// <param name="groupId">The ID of the group to update.</param>
        /// <param name="newName">The new name for the group.</param>
        /// <returns>A standard service response containing the updated group.</returns>
        Task<ServiceResponseDTO<GroupResponseDTO>> UpdateGroupNameAsync(int groupId, string newName);

        /// <summary>
        /// Deletes a group by ID and name, validating the user performing the action.
        /// </summary>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <param name="name">The name of the group to confirm deletion.</param>
        /// <param name="userId">The ID of the group creator or authorized user.</param>
        /// <param name="currentUserId">The ID of the currently authenticated user.</param>
        /// <returns>A standard service response containing the deleted group (if applicable).</returns>
        Task<ServiceResponseDTO<GroupResponseDTO>> DeleteGroupAsync(int groupId, string name, int userId, int currentUserId);

        /// <summary>
        /// Retrieves all groups and their members for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting group information.</param>
        /// <returns>A list of group details.</returns>
        Task<ServiceResponseDTO<List<GroupResponseDTO>>> GetAllGroupsAsync(int userId);

        /// <summary>
        /// Retrieves details of a specific group by its ID.
        /// </summary>
        /// <param name="groupId">The ID of the group to retrieve.</param>
        /// <returns>The group details if found; otherwise, null.</returns>
        Task<ServiceResponseDTO<GroupResponseDTO>> GetGroupByIdAsync(int groupId);
    }


}
