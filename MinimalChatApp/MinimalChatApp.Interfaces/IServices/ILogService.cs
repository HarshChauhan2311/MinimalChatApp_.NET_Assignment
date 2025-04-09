using Microsoft.AspNetCore.Mvc;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    public interface ILogService
    {
        Task<IActionResult> GetLogsAsync(DateTime? startTime, DateTime? endTime);
    }
}
