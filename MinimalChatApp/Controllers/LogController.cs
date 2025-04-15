using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.BAL.IServices;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LogController : ControllerBase
    {
        #region Private Variables
        private readonly ILogService _logService;
        #endregion

        #region Constructors 
        public LogController(ILogService logService)
        {
            _logService = logService;
        }
        #endregion

        #region Public methods
        [HttpGet("log")]
        public async Task<IActionResult> GetLogsAsync([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
        {
            return await _logService.GetLogsAsync(startTime, endTime);
        }
        #endregion
    }
}
