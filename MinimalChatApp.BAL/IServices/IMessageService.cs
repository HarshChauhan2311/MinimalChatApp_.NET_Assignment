using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Service for handling messages between users and within groups.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Sends a private message from one user to another.
        /// </summary>
        /// <param name="senderId">The ID of the user sending the message.</param>
        /// <param name="request">The message request containing recipient and message content.</param>
        /// <returns>A response object with metadata and the sent message.</returns>
        Task<ServiceResponseDTO<SentMessageDTO>> SendMessageAsync(int senderId, MessageRequestDTO request);

        /// <summary>
        /// Edits an existing message sent by a user.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to edit the message.</param>
        /// <param name="messageId">The ID of the message to be edited.</param>
        /// <param name="request">The updated message content.</param>
        /// <returns>A response object indicating success or failure, with status message and code.</returns>
        Task<ServiceResponseDTO<string>> EditMessageAsync(int userId, int messageId, EditMessageRequestDTO request);

        /// <summary>
        /// Deletes a message sent by a user.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to delete the message.</param>
        /// <param name="messageId">The ID of the message to be deleted.</param>
        /// <returns>A response object indicating success or failure, with status message and code.</returns>
        Task<ServiceResponseDTO<string>> DeleteMessageAsync(int userId, int messageId);

        /// <summary>
        /// Retrieves the conversation history between two users.
        /// </summary>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="otherUserId">The ID of the other participant in the conversation.</param>
        /// <param name="before">Optional timestamp to retrieve messages sent before this time.</param>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="sort">Sort order of messages ("asc" or "desc").</param>
        /// <returns>A response object containing the conversation messages.</returns>
        Task<ServiceResponseDTO<List<ConversationMessageDTO>>> GetConversationHistoryAsync(int userId, int otherUserId, DateTime? before, int count, string sort);

        /// <summary>
        /// Searches conversations for messages matching the query string.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="query">The keyword or phrase to search within conversations.</param>
        /// <returns>A response object containing the matched conversations or messages.</returns>
        Task<ServiceResponseDTO<List<ConversationMessageDTO>>> SearchConversationsAsync(int userId, string query);

        /// <summary>
        /// Sends a message to a group.
        /// </summary>
        /// <param name="senderId">The ID of the user sending the group message.</param>
        /// <param name="groupId">The ID of the target group.</param>
        /// <param name="content">The message content.</param>
        /// <returns>A response object indicating success or failure, with status message and code.</returns>
        Task<ServiceResponseDTO<MessageDTO?>> SendGroupMessageAsync(int senderId, int groupId, string content, string? fileUrl, string? contentType);

        /// <summary>
        /// Retrieves messages from a group conversation.
        /// </summary>
        /// <param name="userId">The ID of the requesting user.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="before">Timestamp to retrieve messages sent before this date and time.</param>
        /// <param name="count">Maximum number of messages to retrieve.</param>
        /// <param name="sort">Sort order of the messages ("asc" or "desc").</param>
        /// <returns>A response object containing success status, messages, and status code.</returns>
        Task<ServiceResponseDTO<List<MessageDTO>>> GetGroupMessagesAsync(int userId, int groupId, DateTime before, int count, string sort);

        /// <summary>
        /// Searches group messages for a specific keyword.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="groupId">The ID of the group where the search is performed.</param>
        /// <param name="keyword">The keyword to search within group messages.</param>
        /// <returns>A response object containing success status, matching messages, and status code.</returns>
        Task<ServiceResponseDTO<List<MessageDTO>>> SearchGroupMessagesAsync(int userId, int groupId, string keyword);
    }


}
