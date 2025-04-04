using MinimalChatApp.Models;

namespace MinimalChatApp.Interfaces
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
    }
}
