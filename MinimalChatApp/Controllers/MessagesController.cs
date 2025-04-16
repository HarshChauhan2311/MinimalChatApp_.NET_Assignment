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

            return Ok(result.Data);
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessageAsync(int messageId, [FromBody] EditMessageRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Message content cannot be empty." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var response = await _messageService.EditMessageAsync(userId, messageId, request);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            return Ok(response.Data); // contains IsSuccess, Message, StatusCode
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessageAsync(int messageId)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var response = await _messageService.DeleteMessageAsync(userId, messageId);

            return Ok(response.Data); // contains IsSuccess, Message, StatusCode
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalConversationHistoryAsync(
          [FromBody] PersonalChatHistoryRequestDTO request)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return Unauthorized(new { error = "Unauthorized access." });

            if (request.Count <= 0 || (request.Sort.ToLower() != "asc" && request.Sort.ToLower() != "desc"))
                return BadRequest(new { error = "Invalid request parameters." });

            var result = await _messageService.GetConversationHistoryAsync(currentUserId, request.UserId, request.Before, request.Count, request.Sort);
            if (result != null && result.GetType().GetProperty("error") != null)
            {
                var errorValue = result.GetType().GetProperty("error")?.GetValue(result);
                if (errorValue != null)
                    return BadRequest(result);
            }

            // Assuming result.Data is a single SentMessageDTO object
            // Assuming result.Data is a List<SentMessageDTO> or IEnumerable<SentMessageDTO>
            var response = new
            {
                messages = result.Data // Directly assign the list of objects
            };

            return Ok(response);
        }
        #endregion

    }

}
