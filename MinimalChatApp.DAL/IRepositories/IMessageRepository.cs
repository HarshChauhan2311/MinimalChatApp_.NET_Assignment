using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.IRepositories
{
    public interface IMessageRepository
    {
        /// <summary>
        /// Sends a new message asynchronously.
        /// </summary>
        /// <param name="senderId">The ID of the sender.</param>
        /// <param name="receiverId">The ID of the receiver.</param>
        /// <param name="content">The content of the message.</param>
        /// <returns>
        /// The sent <see cref="Message"/> object with generated metadata (e.g., ID, timestamp).
        /// </returns>
        Task<Message> SendMessageAsync(int senderId, int receiverId, string content);

        /// <summary>
        /// Retrieves a message by its unique identifier asynchronously.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message.</param>
        /// <returns>
        /// A <see cref="Message"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Message?> GetByIdAsync(int messageId);

        /// <summary>
        /// Updates an existing message asynchronously.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> object with updated content.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task UpdateAsync(Message message);

        /// <summary>
        /// Deletes a message asynchronously.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> object to be deleted.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task DeleteAsync(Message message);

        /// <summary>
        /// Retrieves the conversation history between the current user and a target user.
        /// </summary>
        /// <param name="currentUserId">The ID of the current user.</param>
        /// <param name="targetUserId">The ID of the user to retrieve the conversation with.</param>
        /// <param name="before">Fetch messages before this timestamp.</param>
        /// <param name="count">Number of messages to retrieve.</param>
        /// <param name="sort">Sorting order: "asc" or "desc".</param>
        /// <returns>
        /// A list of <see cref="Message"/> objects representing the conversation history.
        /// </returns>
        Task<List<Message>> GetConversationAsync(int currentUserId, int targetUserId, DateTime before, int count, string sort);

        /// <summary>
        /// Searches messages for a specific user by a search query.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="query">The search keyword or phrase.</param>
        /// <returns>A list of messages matching the search criteria.</returns>
        Task<List<Message>> SearchMessagesAsync(int userId, string query);

        /// <summary>
        /// Sends a message to a specified group.
        /// </summary>
        /// <param name="senderId">The ID of the user sending the message.</param>
        /// <param name="groupId">The ID of the group to send the message to.</param>
        /// <param name="content">The content of the message.</param>
        /// <returns>The sent message entity if successful; otherwise, null.</returns>
        Task<Message?> SendGroupMessageAsync(int senderId, int groupId, string content);

        /// <summary>
        /// Retrieves messages from a group before a specified time, limited to a count and ordered by sort preference.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="before">The timestamp to retrieve messages before.</param>
        /// <param name="count">The maximum number of messages to retrieve.</param>
        /// <param name="sort">The sort order (e.g., "asc", "desc").</param>
        /// <returns>A list of group messages.</returns>
        Task<List<Message>> GetGroupMessagesAsync(int groupId, DateTime before, int count, string sort);

        /// <summary>
        /// Searches for group messages containing a specific keyword, scoped to the requesting user.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="keyword">The keyword to search for in group messages.</param>
        /// <returns>A list of group messages that match the keyword.</returns>
        Task<List<Message>> SearchGroupMessagesAsync(int groupId, int userId, string keyword);

    }
}
