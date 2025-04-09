using MinimalChatApp.DTOs;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<object> SendMessageAsync(int senderId, MessageRequest request);
        Task<object> EditMessageAsync(int userId, int messageId, EditMessageRequest request);
        Task<object> DeleteMessageAsync(int userId, int messageId);
        Task<object> GetConversationHistoryAsync(int userId, int otherUserId, DateTime? before, int count, string sort);
        Task<object> SearchConversationsAsync(int userId, string query);
    }
}
