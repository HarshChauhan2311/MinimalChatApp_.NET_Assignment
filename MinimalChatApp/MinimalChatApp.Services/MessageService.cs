using MinimalChatApp.DTOs;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;

namespace MinimalChatApp.MinimalChatApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IErrorLogService _errorLogger;

        public MessageService(IMessageRepository messageRepo, IUserRepository userRepo, IErrorLogService errorLogger)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _errorLogger = errorLogger;
        }

        public async Task<object> SendMessageAsync(int senderId, MessageRequest request)
        {
            var receiver = await _userRepo.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
                return new { error = "Receiver not found." };

            try
            {
                var message = await _messageRepo.SendMessageAsync(senderId, request.ReceiverId, request.Content);
                if (message == null)
                    return new { error = "Message sending failed due to validation errors" };

                return new
                {
                    messageId = message.Id,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    content = message.Content,
                    timestamp = message.Timestamp
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new { error = ex.Message };
            }
        }

        public async Task<object> EditMessageAsync(int userId, int messageId, EditMessageRequest request)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return new { error = "Message not found." };

            if (message.SenderId != userId)
                return new { error = "Unauthorized access." };

            try
            {
                message.Content = request.Content;
                await _messageRepo.UpdateAsync(message);

                return new
                {
                    messageId = message.Id,
                    content = message.Content,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    timestamp = message.Timestamp
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new { error = ex.Message };
            }
        }

        public async Task<object> DeleteMessageAsync(int userId, int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return new { error = "Message not found." };

            if (message.SenderId != userId)
                return new { error = "Unauthorized access." };

            try
            {
                await _messageRepo.DeleteAsync(message);
                return new { message = "Message deleted successfully." };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new { error = ex.Message };
            }
        }

        public async Task<object> GetConversationHistoryAsync(int userId, int otherUserId, DateTime? before, int count, string sort)
        {
            var timestamp = before ?? DateTime.UtcNow;

            try
            {
                var messages = await _messageRepo.GetConversationAsync(userId, otherUserId, timestamp, count, sort.ToLower());
                if (messages == null || !messages.Any())
                    return new { error = "No conversation history found." };

                return new
                {
                    messages = messages.Select(m => new
                    {
                        id = m.Id,
                        senderId = m.SenderId,
                        receiverId = m.ReceiverId,
                        content = m.Content,
                        timestamp = m.Timestamp
                    })
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new { error = ex.Message };
            }
        }

        public async Task<object> SearchConversationsAsync(int userId, string query)
        {
            try
            {
                var messages = await _messageRepo.SearchMessagesAsync(userId, query);
                return new
                {
                    messages = messages.Select(m => new
                    {
                        id = m.Id,
                        senderId = m.SenderId,
                        receiverId = m.ReceiverId,
                        content = m.Content,
                        timestamp = m.Timestamp
                    })
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new { error = ex.Message };
            }
        }
    }
}
