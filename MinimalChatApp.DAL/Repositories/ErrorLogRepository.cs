using MinimalChatApp.DAL.Data;
using MinimalChatApp.DAL.IRepositories;
using MinimalChatApp.DAL.Repositories;
using MinimalChatApp.Entity;

namespace MinimalChatApp.BAL.Services
{
    public class ErrorLogRepository : GenericRepository<ErrorLog>, IErrorLogRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        #endregion

        #region Constructors 
        public ErrorLogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        #endregion

        #region Public Method
        public async Task LogErrorAsync(ErrorLog log)
        {
            await _context.ErrorLogs.AddAsync(log);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
