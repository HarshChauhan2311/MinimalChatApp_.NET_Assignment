using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class LogController : ControllerBase
    {
        #region Private Variables
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public LogController(AppDbContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        [HttpGet("log")]
        [Authorize]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
        {
            var start = startTime ?? DateTime.UtcNow.AddMinutes(-5);
            var end = endTime ?? DateTime.UtcNow;

            try
            {
                if (start > end)
                    return BadRequest(new { error = "StartTime must be earlier than EndTime" });

                var logs = await _context.RequestLogs
                    .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                    .ToListAsync();

                if (!logs.Any())
                    return NotFound(new { error = "No logs found" });

                return Ok(new { Logs = logs });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }
        #endregion
    }
}
