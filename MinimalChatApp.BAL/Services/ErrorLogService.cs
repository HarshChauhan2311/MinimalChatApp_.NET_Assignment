using MinimalChatApp.DAL.Data;
using MinimalChatApp.Entity;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;

namespace MinimalChatApp.BAL.Services
{
    public class ErrorLogService : IErrorLogService
    {
        #region Private Variables
        private readonly IErrorLogRepository _errorLogRepository;
        #endregion

        #region Constructors 
        public ErrorLogService(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
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

            await _errorLogRepository.LogErrorAsync(error);
            await _errorLogRepository.SaveChangesAsync();
        }
        #endregion
    }
}
