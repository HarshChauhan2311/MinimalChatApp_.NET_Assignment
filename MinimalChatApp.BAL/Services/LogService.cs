

using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;

namespace MinimalChatApp.BAL.Services
{
    public class LogService : ILogService
    {
        private readonly IErrorLogService _errorLogService;
        private readonly ILogRepository _logRepository;

        public LogService(IErrorLogService errorLogService,ILogRepository logRepository)
        {
            _logRepository = logRepository;
            _errorLogService = errorLogService;
        }
        public async Task<IActionResult> GetLogsAsync(DateTime? startTime, DateTime? endTime)
        {
            var start = startTime ?? DateTime.UtcNow.AddMinutes(-5);
            var end = endTime ?? DateTime.UtcNow;
            try
            {
                if (start > end)
                    return new BadRequestObjectResult(new { error = "StartTime must be earlier than EndTime" });

                var logs = await _logRepository.GetLogsAsync(start, end);

                if (!logs.Any())
                    return new NotFoundObjectResult(new { error = "No logs found" });

                return new OkObjectResult(new { Logs = logs });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
