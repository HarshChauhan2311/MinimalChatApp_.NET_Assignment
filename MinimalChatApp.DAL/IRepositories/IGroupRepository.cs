using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.IRepositories
{
    /// <summary>
    /// Defines methods for managing groups and group members in the repository.
    /// </summary>
    public interface IGroupRepository
    {
        /// <summary>
        /// Retrieves a group by its name.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <returns>The group entity if found; otherwise, null.</returns>
        Task<Group?> GetByNameAsync(string name);

        /// <summary>
        /// Retrieves a group by its ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>The group entity if found; otherwise, null.</returns>
        Task<Group?> GetByIdAsync(int groupId);

        /// <summary>
        /// Retrieves a group by its ID (alternate method).
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>The group entity if found; otherwise, null.</returns>
        Task<Group?> GetGroupByIdAsync(int groupId);

        /// <summary>
        /// Retrieves a group by both its ID and name.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="name">The name of the group.</param>
        /// <returns>The group entity if found; otherwise, null.</returns>
        Task<Group?> GetByIdAndNameAsync(int groupId, string name);

        /// <summary>
        /// Checks if a group name already exists, optionally excluding a specific group ID.
        /// </summary>
        /// <param name="name">The name of the group to check.</param>
        /// <param name="excludeGroupId">An optional group ID to exclude from the check.</param>
        /// <returns>True if the group name exists; otherwise, false.</returns>
        Task<bool> GroupNameExistsAsync(string name, int? excludeGroupId = null);

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="userId">The ID of the user creating the group.</param>
        /// <returns>The created group entity.</returns>
        Task<Group?> CreateGroupAsync(string groupName, int userId);

        /// <summary>
        /// Updates the name of an existing group.
        /// </summary>
        /// <param name="group">The group to update.</param>
        /// <param name="newName">The new name for the group.</param>
        /// <returns>The updated group entity.</returns>
        Task<Group?> UpdateGroupNameAsync(Group group, string newName);

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="group">The group to delete.</param>
        /// <returns>True if the group was deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteGroupAsync(Group group);

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

        /// <summary>
        /// Checks if a group exists by its ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>True if the group exists; otherwise, false.</returns>
        Task<bool> GroupExistsAsync(int groupId);

        /// <summary>
        /// Checks if a user is a member of a group.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>True if the user is a member; otherwise, false.</returns>
        Task<bool> IsUserGroupMemberAsync(int userId, int groupId);

        /// <summary>
        /// Retrieves all groups with their members for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of groups with their members.</returns>
        Task<List<Group>> GetAllGroupsWithMembersAsync(int userId);
    }

}
