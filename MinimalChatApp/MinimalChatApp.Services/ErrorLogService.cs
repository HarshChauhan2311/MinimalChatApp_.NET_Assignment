using MinimalChatApp.Data;
using MinimalChatApp.Interfaces;
using MinimalChatApp.Models;

namespace MinimalChatApp.Services
{
    public class ErrorLogService : IErrorLogService
    {
        #region Private Variables
        private readonly AppDbContext _context;
        #endregion

        #region Constructors 
        public ErrorLogService(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Public Method
        public async Task LogAsync(Exception ex)
        {
            var error = new ErrorLog
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                InnerException = ex.InnerException?.Message
            };

            _context.ErrorLogs.Add(error);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
