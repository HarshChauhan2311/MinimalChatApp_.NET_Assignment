using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Models;
using MinimalChatApp.Services;

namespace MinimalChatApp.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public MessageRepository(AppDbContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
        {
            try
            {
                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    Timestamp = DateTime.UtcNow
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                return message;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null; // Let the controller handle the 500 response
            }
        }
        public async Task<Message?> GetByIdAsync(int messageId)
        {
            try
            {
                return await _context.Messages.FindAsync(messageId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null; // Let the controller handle the 500 response
            }
        }
        public async Task UpdateAsync(Message message)
        {
            try
            {
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                throw; // Let the controller handle the 500 response
            }
        }
        public async Task DeleteAsync(Message message)
        {
            try
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                throw; // Let the controller handle the 500 response
            }
        }
        public async Task<List<Message>> GetConversationAsync(int currentUserId, int targetUserId, DateTime before, int count, string sort)
        {
            try
            {
                var query = _context.Messages
                    .Where(m =>
                        (m.SenderId == currentUserId && m.ReceiverId == targetUserId ||
                         m.SenderId == targetUserId && m.ReceiverId == currentUserId ||
                         m.SenderId == currentUserId) && m.Timestamp < before);

                query = sort.ToLower() == "asc"
                    ? query.OrderByDescending(m => m.Timestamp)
                    : query.OrderBy(m => m.Timestamp);

                return await query.Take(count).ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new List<Message>(); // Let the controller handle the 500 response
            }
        }

        public async Task<List<Message>> SearchMessagesAsync(int userId, string query)
        {
            try
            {
                return await _context.Messages
                .Where(m =>
                    (m.SenderId == userId || m.ReceiverId == userId) &&
                    m.Content.Contains(query))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<Message?> SendGroupMessageAsync(int senderId, int groupId, string content)
        {

            try
            {
                var message = new Message
                {
                    SenderId = senderId,
                    GroupId = groupId,
                    Content = content,
                    Timestamp = DateTime.UtcNow
                };
                await _context.Messages.AddAsync(message);
                var saved = await _context.SaveChangesAsync() > 0;

                return saved ? message : null;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }

        }

        public async Task<List<Message>> GetGroupMessagesAsync(int groupId, DateTime before, int count, string sort)
        {
            try
            {
                var query = _context.Messages
                .Where(m => m.GroupId == groupId && m.Timestamp < before);

                query = sort.ToLower() == sort
                    ? query.OrderByDescending(m => m.Timestamp)
                    : query.OrderBy(m => m.Timestamp);

                return await query.Take(count).ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }

        public async Task<List<Message>> SearchGroupMessagesAsync(int groupId, int userId, string keyword)
        {
            try
            {
                return await _context.Messages
                .Where(m =>
                    m.GroupId == groupId &&
                    (m.SenderId == userId || m.ReceiverId == userId || m.ReceiverId == null) &&
                    m.Content.Contains(keyword))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }




        #endregion
    }
}
