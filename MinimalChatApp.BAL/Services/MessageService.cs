using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DTO;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.Entity;
using MinimalChatApp.DAL.IRepositories;

namespace MinimalChatApp.BAL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IUserRepository _userRepo;
        private readonly IErrorLogService _errorLogger;

        public MessageService(IMessageRepository messageRepo, IUserRepository userRepo, IErrorLogService errorLogger, IGroupRepository groupRepo)
        {
            _messageRepo = messageRepo;
            _groupRepo = groupRepo;
            _userRepo = userRepo;
            _errorLogger = errorLogger;
        }

        public async Task<object> SendMessageAsync(int senderId, MessageRequestDTO request)
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

        public async Task<(bool isSuccess, string? error, Message? message, int statusCode)> EditMessageAsync(int userId, int messageId, EditMessageRequestDTO request)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return (false, "Message not found.", null, 404);

            if (message.SenderId != userId)
                return (false, "Unauthorized access.", null, 401);

            try
            {
                message.Content = request.Content;
                await _messageRepo.UpdateAsync(message);

                return (true, null, message, 200);
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return (false, "Internal server error: " + ex.Message, null, 400);
            }
        }

        public async Task<(bool isSuccess, string? error, string? message, int statusCode)> DeleteMessageAsync(int userId, int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return (false, "Message not found.", null, 404);

            if (message.SenderId != userId)
                return (false, "Unauthorized access.", null, 401);

            try
            {
                await _messageRepo.DeleteAsync(message);
                return (true, null, "Message deleted successfully.", 200);

            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return (false, "Internal server error: " + ex.Message, null, 400);
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

        public async Task<(bool isSuccess, string? error, Message? message)> SendGroupMessageAsync(int senderId, int groupId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return (false, "Message content cannot be empty.", null);

            try
            {
                var message = await _messageRepo.SendGroupMessageAsync(senderId, groupId, content);

            return message != null
                ? (true, null, message)
                : (false, "Failed to send message.", null);
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return (false, $"Internal server error: {ex.Message}", null);
            }
        }

        public async Task<(bool isSuccess, string? error, List<Message>? messages, int statusCode)> GetGroupMessagesAsync(int userId, int groupId, DateTime before, int count, string sort)
        {
            if (!await _groupRepo.GroupExistsAsync(groupId))
                return (false, "Group not found.", null, 404);

            if (!await _groupRepo.IsUserGroupMemberAsync(userId, groupId))
                return (false, "Unauthorized access.", null, 401);

            try
            {
                var messages = await _messageRepo.GetGroupMessagesAsync(groupId, before, count, sort);
                if (messages == null || !messages.Any())
                    return (false, "No messages found.", null, 404);

                return (true, null, messages, 200);
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return (false, $"Internal server error: {ex.Message}", null, 500);
            }
        }

        public async Task<(bool isSuccess, string? error, List<Message>? messages, int statusCode)> SearchGroupMessagesAsync(int userId, int groupId, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return (false, "Search keyword cannot be empty.", null, 400);

            var groupExists = await _groupRepo.GroupExistsAsync(groupId);
            if (!groupExists)
                return (false, "Group not found.", null, 404);

            var isMember = await _groupRepo.IsUserGroupMemberAsync(userId, groupId);
            if (!isMember)
                return (false, "Unauthorized access.", null, 401);

            try
            {
                var messages = await _messageRepo.SearchGroupMessagesAsync(groupId, userId, keyword);
                return (true, null, messages, 200);
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return (false, $"Internal server error: {ex.Message}", null, 500);
            }
        }



    }
}
