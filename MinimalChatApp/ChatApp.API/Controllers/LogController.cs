using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.ChatApp.Infrastructure.Data;

namespace MinimalChatApp.ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
        {
            var start = startTime ?? DateTime.UtcNow.AddMinutes(-5);
            var end = endTime ?? DateTime.UtcNow;

            if (start > end)
                return BadRequest(new { error = "StartTime must be earlier than EndTime" });

            var logs = await _context.RequestLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                .ToListAsync();

            if (!logs.Any())
                return NotFound(new { error = "No logs found" });

            return Ok(new { Logs = logs });
        }
    }
}
