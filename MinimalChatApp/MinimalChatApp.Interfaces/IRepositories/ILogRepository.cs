using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IRepositories
{
    public interface ILogRepository
    {
        Task<List<RequestLog>> GetLogsAsync(DateTime startTime, DateTime endTime);
    }
}
