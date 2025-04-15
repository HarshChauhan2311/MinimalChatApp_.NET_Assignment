using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        //private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public MessageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        #endregion

        #region Public methods
        public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
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
        public async Task<Message?> GetByIdAsync(int messageId)
        {

            return await _context.Messages.FindAsync(messageId);

        }
        public async Task UpdateAsync(Message message)
        {

            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

        }
        public async Task DeleteAsync(Message message)
        {

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

        }
        public async Task<List<Message>> GetConversationAsync(int currentUserId, int targetUserId, DateTime before, int count, string sort)
        {
            var query = _context.Messages
             .AsNoTracking() // ✅ No tracking since you're just reading data
             .Where(m =>
                 ((m.SenderId == currentUserId && m.ReceiverId == targetUserId) ||
                  (m.SenderId == targetUserId && m.ReceiverId == currentUserId))
                 && m.Timestamp < before);

            query = sort.ToLower() == "asc"
                ? query.OrderBy(m => m.Timestamp)
                : query.OrderByDescending(m => m.Timestamp);

            return await query.Take(count).ToListAsync();

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

        public async Task<Message?> SendGroupMessageAsync(int senderId, int groupId, string content)
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

        public async Task<List<Message>> GetGroupMessagesAsync(int groupId, DateTime before, int count, string sort)
        {

            var query = _context.Messages
            .Where(m => m.GroupId == groupId && m.Timestamp < before);

            query = sort.ToLower() == sort
                ? query.OrderByDescending(m => m.Timestamp)
                : query.OrderBy(m => m.Timestamp);

            return await query.Take(count).ToListAsync();

        }

        public async Task<List<Message>> SearchGroupMessagesAsync(int groupId, int userId, string keyword)
        {

            return await _context.Messages
            .Where(m =>
                m.GroupId == groupId &&
                (m.SenderId == userId || m.ReceiverId == userId || m.ReceiverId == null) &&
                m.Content.Contains(keyword))
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();

        }




        #endregion
    }
}
