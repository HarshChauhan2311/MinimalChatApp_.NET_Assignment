using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Models;

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
                         m.SenderId == targetUserId && m.ReceiverId == currentUserId)
                        && m.Timestamp < before);

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
            return await _context.Messages
                .Where(m =>
                    (m.SenderId == userId || m.ReceiverId == userId) &&
                    m.Content.Contains(query))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }

        #endregion
    }
}
