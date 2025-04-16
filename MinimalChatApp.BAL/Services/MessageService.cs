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

        public async Task<ServiceResponseDTO<SentMessageDTO>> SendMessageAsync(int senderId, MessageRequestDTO request)
        {
            var receiver = await _userRepo.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
            {
                return new ServiceResponseDTO<SentMessageDTO>
                {
                    IsSuccess = false,
                    Error = "Receiver not found.",
                    StatusCode = 404
                };
            }

            try
            {
                var message = await _messageRepo.SendMessageAsync(senderId, request.ReceiverId, request.Content);

                if (message == null)
                {
                    return new ServiceResponseDTO<SentMessageDTO>
                    {
                        IsSuccess = false,
                        Error = "Message sending failed.",
                        StatusCode = 400
                    };
                }

                var messageDto = new SentMessageDTO
                {
                    MessageId = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    Timestamp = message.Timestamp
                };

                return new ServiceResponseDTO<SentMessageDTO>
                {
                    IsSuccess = true,
                    Data = messageDto,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<SentMessageDTO>
                {
                    IsSuccess = false,
                    Error = ex.Message,
                    StatusCode = 500
                };
            }
        }



        public async Task<ServiceResponseDTO<string>> EditMessageAsync(int userId, int messageId, EditMessageRequestDTO request)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = "Message not found.", StatusCode = 404 };

            if (message.SenderId != userId)
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = "Unauthorized access.", StatusCode = 401 };

            try
            {
                message.Content = request.Content;
                await _messageRepo.UpdateAsync(message);

                return new ServiceResponseDTO<string> { IsSuccess = true, Data = "Message edited successfully.", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<string>> DeleteMessageAsync(int userId, int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = "Message not found.", StatusCode = 404 };

            if (message.SenderId != userId)
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = "Unauthorized access.", StatusCode = 401 };

            try
            {
                await _messageRepo.DeleteAsync(message);
                return new ServiceResponseDTO<string> { IsSuccess = true, Data = "Message deleted successfully.", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<string> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }


        public async Task<ServiceResponseDTO<List<ConversationMessageDTO>>> GetConversationHistoryAsync(int userId, int otherUserId, DateTime? before, int count, string sort)
        {
            try
            {
                var timestamp = before ?? DateTime.UtcNow;
                var messages = await _messageRepo.GetConversationAsync(userId, otherUserId, timestamp, count, sort.ToLower());

                if (messages == null || !messages.Any())
                    return new ServiceResponseDTO<List<ConversationMessageDTO>> { IsSuccess = false, Error = "No conversation history found.", StatusCode = 404 };

                var response = messages.Select(m => new ConversationMessageDTO
                {
                    MessageId =  m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList();

                return new ServiceResponseDTO<List<ConversationMessageDTO>> { IsSuccess = true, Data = response, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<List<ConversationMessageDTO>> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<List<ConversationMessageDTO>>> SearchConversationsAsync(int userId, string query)
        {
            try
            {
                var messages = await _messageRepo.SearchMessagesAsync(userId, query);
                var response = messages.Select(m => new ConversationMessageDTO
                {
                    MessageId = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                }).ToList();

                return new ServiceResponseDTO<List<ConversationMessageDTO>> { IsSuccess = true, Data = response, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<List<ConversationMessageDTO>> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<MessageDTO>> SendGroupMessageAsync(int senderId, int groupId, string content, string? fileUrl, string? contentType)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new ServiceResponseDTO<MessageDTO>
                {
                    IsSuccess = false,
                    Error = "Message content cannot be empty.",
                    StatusCode = 400
                };

            try
            {
                var message = await _messageRepo.SendGroupMessageAsync(senderId, groupId, content, fileUrl, contentType);

                if (message == null)
                {
                    return new ServiceResponseDTO<MessageDTO>
                    {
                        IsSuccess = false,
                        Error = "Failed to send message.",
                        StatusCode = 500
                    };
                }

                var messageDto = new MessageDTO
                {
                    MessageId = message.Id,
                    SenderId = message.SenderId,
                    GroupId = message.GroupId,
                    Content = message.Content,
                    FileUrl = message.FileUrl,
                    ContentType = message.ContentType,
                    Timestamp = message.Timestamp
                };

                return new ServiceResponseDTO<MessageDTO>
                {
                    IsSuccess = true,
                    Data = messageDto,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<MessageDTO>
                {
                    IsSuccess = false,
                    Error = ex.Message,
                    StatusCode = 500
                };
            }
        }


        public async Task<ServiceResponseDTO<List<MessageDTO>>> GetGroupMessagesAsync(int userId, int groupId, DateTime before, int count, string sort)
        {
            if (!await _groupRepo.GroupExistsAsync(groupId))
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "Group not found.", StatusCode = 404 };

            if (!await _groupRepo.IsUserGroupMemberAsync(userId, groupId))
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "Unauthorized access.", StatusCode = 401 };

            try
            {
                var messages = await _messageRepo.GetGroupMessagesAsync(groupId, before, count, sort);
                if (messages == null || !messages.Any())
                    return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "No messages found.", StatusCode = 404 };

                var messageDtos = messages.Select(m => new MessageDTO
                {
                    MessageId = m.Id,
                    SenderId = m.SenderId,
                    GroupId = m.GroupId,
                    Content = m.Content,
                    FileUrl = m.FileUrl,
                    ContentType = m.ContentType,
                    Timestamp = m.Timestamp
                }).ToList();

                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = true, Data = messageDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<ServiceResponseDTO<List<MessageDTO>>> SearchGroupMessagesAsync(int userId, int groupId, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "Search keyword cannot be empty.", StatusCode = 400 };

            if (!await _groupRepo.GroupExistsAsync(groupId))
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "Group not found.", StatusCode = 404 };

            if (!await _groupRepo.IsUserGroupMemberAsync(userId, groupId))
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = "Unauthorized access.", StatusCode = 401 };

            try
            {
                var messages = await _messageRepo.SearchGroupMessagesAsync(groupId, userId, keyword);
                var messageDtos = messages.Select(m => new MessageDTO
                {
                    MessageId = m.Id,
                    SenderId = m.SenderId,
                    GroupId = m.GroupId,
                    Content = m.Content,
                    FileUrl = m.FileUrl,
                    ContentType = m.ContentType,
                    Timestamp = m.Timestamp
                }).ToList();

                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = true, Data = messageDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return new ServiceResponseDTO<List<MessageDTO>> { IsSuccess = false, Error = ex.Message, StatusCode = 500 };
            }
        }
    }
}
