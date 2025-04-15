using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DTO;
using MinimalChatApp.Hubs;

namespace MinimalChatApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class MessagesController : ControllerBase
    {
        #region Private Variables
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserConnectionManagerService _connectionManager;
        #endregion

        #region Constructors 
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext, IUserConnectionManagerService connectionManager)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }
        #endregion

        #region Public methods
        [HttpPost("send")]
        public async Task<IActionResult> SendRealtimeMessageAsync([FromBody] SendMessageDto dto)
        {
            //await _hubContext.Clients.User(dto.ToUser).SendAsync("ReceiveMessage", User.Identity?.Name, dto.Message);
            //return Ok(new { status = "sent" });
            // Create a message object that matches our client's expected format
            var message = new
            {
                to = dto.ToUser,
                from = User.Identity?.Name,
                message = dto.Message
            };

            // Send the formatted message object
            await _hubContext.Clients.User(dto.ToUser).SendAsync("ReceiveMessage", message);
            return Ok(new { status = "sent" });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequestDTO request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Invalid message data." });

            var senderIdClaim = User.FindFirst("userId");
            if (senderIdClaim == null || !int.TryParse(senderIdClaim.Value, out int senderId))
                return Unauthorized(new { error = "Unauthorized access." });

            var result = await _messageService.SendMessageAsync(senderId, request);
            if (result != null && result.GetType().GetProperty("error") != null)
            {
                var errorValue = result.GetType().GetProperty("error")?.GetValue(result);
                if (errorValue != null)
                    return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessageAsync(int messageId, [FromBody] EditMessageRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Message content cannot be empty." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, message, statusCode) = await _messageService.EditMessageAsync(userId, messageId, request);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new { message });
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessageAsync(int messageId)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, message, statusCode) = await _messageService.DeleteMessageAsync(userId, messageId);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new { message });
        }

        [HttpGet]
        public async Task<IActionResult> GetConversationHistoryAsync(
           [FromQuery] int userId,
           [FromQuery] DateTime? before,
           [FromQuery] int count = 20,
           [FromQuery] string sort = "asc")
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return Unauthorized(new { error = "Unauthorized access." });

            if (count <= 0 || (sort.ToLower() != "asc" && sort.ToLower() != "desc"))
                return BadRequest(new { error = "Invalid request parameters." });

            var result = await _messageService.GetConversationHistoryAsync(currentUserId, userId, before, count, sort);
            if (result != null && result.GetType().GetProperty("error") != null)
            {
                var errorValue = result.GetType().GetProperty("error")?.GetValue(result);
                if (errorValue != null)
                    return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

    }

}
