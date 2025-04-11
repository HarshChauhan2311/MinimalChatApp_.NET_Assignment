using MinimalChatApp.DTOs;
using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    /// <summary>
    /// Service for handling messages between users and within groups.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Sends a message from a user to another user.
        /// </summary>
        /// <param name="senderId">The ID of the message sender.</param>
        /// <param name="request">The message request payload.</param>
        /// <returns>An object representing the sent message and related metadata.</returns>
        Task<object> SendMessageAsync(int senderId, MessageRequest request);

        /// <summary>
        /// Edits a previously sent message.
        /// </summary>
        /// <param name="userId">The ID of the user editing the message.</param>
        /// <param name="messageId">The ID of the message to edit.</param>
        /// <param name="request">The new message content.</param>
        /// <returns>A tuple containing success, error message, updated message, and status code.</returns>
        Task<(bool isSuccess, string? error, Message? message, int statusCode)> EditMessageAsync(int userId, int messageId, EditMessageRequest request);

        /// <summary>
        /// Deletes a specific message for a user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting deletion.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <returns>A tuple indicating success, error message, and status code.</returns>
        Task<(bool isSuccess, string? error, string? message, int statusCode)> DeleteMessageAsync(int userId, int messageId);

        /// <summary>
        /// Retrieves conversation history between two users.
        /// </summary>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="otherUserId">The ID of the other user in the conversation.</param>
        /// <param name="before">Optional timestamp to filter messages before.</param>
        /// <param name="count">Maximum number of messages to retrieve.</param>
        /// <param name="sort">Sort order (e.g., "asc" or "desc").</param>
        /// <returns>An object containing the message history.</returns>
        Task<object> GetConversationHistoryAsync(int userId, int otherUserId, DateTime? before, int count, string sort);

        /// <summary>
        /// Searches for conversations based on a query string.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="query">The search query string.</param>
        /// <returns>An object containing matching conversations or messages.</returns>
        Task<object> SearchConversationsAsync(int userId, string query);

        /// <summary>
        /// Sends a message to a specific group.
        /// </summary>
        /// <param name="senderId">The ID of the user sending the message.</param>
        /// <param name="groupId">The ID of the target group.</param>
        /// <param name="content">The message content.</param>
        /// <returns>A tuple indicating success, error message, and the sent group message.</returns>
        Task<(bool isSuccess, string? error, Message? message)> SendGroupMessageAsync(int senderId, int groupId, string content);

        /// <summary>
        /// Retrieves messages from a group.
        /// </summary>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="before">Timestamp to retrieve messages before.</param>
        /// <param name="count">Number of messages to retrieve.</param>
        /// <param name="sort">Sort order (e.g., "asc", "desc").</param>
        /// <returns>A tuple with success, error message, list of messages, and status code.</returns>
        Task<(bool isSuccess, string? error, List<Message>? messages, int statusCode)> GetGroupMessagesAsync(int userId, int groupId, DateTime before, int count, string sort);

        /// <summary>
        /// Searches messages within a group using a keyword.
        /// </summary>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>A tuple with success, error message, list of matching messages, and status code.</returns>
        Task<(bool isSuccess, string? error, List<Message>? messages, int statusCode)> SearchGroupMessagesAsync(int userId, int groupId, string keyword);
    }

}
