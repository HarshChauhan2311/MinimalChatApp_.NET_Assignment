using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Repositories
{
    public class LogRepository : GenericRepository<RequestLog>, ILogRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        #endregion

        #region Constructors 
        public LogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        #endregion

        #region Public methods
        public async Task<List<RequestLog>> GetLogsAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.RequestLogs
                    .Where(l => l.Timestamp >= startTime && l.Timestamp <= endTime)
                    .ToListAsync();

        }
        #endregion
    }
}
