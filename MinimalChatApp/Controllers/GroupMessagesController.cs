using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DTO;
using MinimalChatApp.Hubs;

namespace MinimalChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class GroupMessagesController : ControllerBase
    {
        #region Private Variables
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserConnectionManagerService _connectionManager;
        #endregion

        #region Constructors 
        public GroupMessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext, IUserConnectionManagerService connectionManager)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }
        #endregion

        #region Public methods
        [HttpPost]
        public async Task<IActionResult> SendGroupMessage([FromBody] SendGroupMessageRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupId <= 0 || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Invalid request data." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int senderId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, message) = await _messageService.SendGroupMessageAsync(senderId, request.GroupId, request.Content);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new
            {
                messageId = message!.Id,
                groupId = message.GroupId,
                senderId = message.SenderId,
                content = message.Content,
                timestamp = message.Timestamp
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupConversationHistory(
            [FromQuery] int groupId,
            [FromQuery] DateTime? before,
            [FromQuery] int count = 20,
            [FromQuery] string sort = "asc")
        {
            // Get userId from JWT token
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            // Validate inputs
            if (groupId <= 0 || (sort.ToLower() != "asc" && sort.ToLower() != "desc"))
                return BadRequest(new { error = "Invalid request parameters." });

            var beforeDate = before ?? DateTime.UtcNow;

            // Call service
            var (isSuccess, error, messages, statusCode) = await _messageService.GetGroupMessagesAsync(
                userId, groupId, beforeDate, count, sort);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new
            {
                messages = messages!.Select(m => new
                {
                    id = m.Id,
                    senderId = m.SenderId,
                    content = m.Content,
                    timestamp = m.Timestamp
                })
            });
        }
        #endregion
    }
}
