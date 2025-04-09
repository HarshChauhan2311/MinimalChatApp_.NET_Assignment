using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Repositories
{
    public class LogRepository : ILogRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public LogRepository(AppDbContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        public async Task<List<RequestLog>> GetLogsAsync(DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _context.RequestLogs
                    .Where(l => l.Timestamp >= startTime && l.Timestamp <= endTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                throw;
            }
        }
        #endregion
    }
}
