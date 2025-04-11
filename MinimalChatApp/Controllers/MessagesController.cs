using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.DTOs;
using MinimalChatApp.Hubs;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.DTOs;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;

namespace MinimalChatApp.Controllers
{

    [Route("api")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        #region Private Variables
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserConnectionManager _connectionManager;
        #endregion

        #region Constructors 
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext, IUserConnectionManager connectionManager)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }
        #endregion

        #region Public methods
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest request)
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessageAsync(int messageId, [FromBody] EditMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Message content cannot be empty." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, message, statusCode) = await _messageService.EditMessageAsync(userId, messageId, request);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new
            {
                message = "Message Edited successfully.",
                messageId = message!.Id,
                content = message.Content,
                senderId = message.SenderId,
                receiverId = message.ReceiverId,
                groupId = message.GroupId,
                timestamp = message.Timestamp
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("messages/{messageId}")]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("messages")]
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

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet("conversation/search")]
        //public async Task<IActionResult> SearchConversationsAsync([FromQuery] string query)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return BadRequest(new { error = "Query parameter is required." });

        //    var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        //    var result = await _messageService.SearchConversationsAsync(userId, query);

        //    return Ok(result);
        //}

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("groupmessages")]
        public async Task<IActionResult> SendGroupMessage([FromBody] SendGroupMessageRequest request)
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("groupmessages")]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("conversation/search")]
        public async Task<IActionResult> SearchGroupConversations(
        [FromQuery] int groupId,
        [FromQuery] string message)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            if (groupId <= 0 || string.IsNullOrWhiteSpace(message))
                return BadRequest(new { error = "Invalid request parameters." });

            var (isSuccess, error, messages, statusCode) = await _messageService.SearchGroupMessagesAsync(userId, groupId, message);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new
            {
                messages = messages!.Select(m => new
                {
                    id = m.GroupId,
                    messageId = m.Id,
                    senderId = m.SenderId,
                    receiverId = m.ReceiverId,
                    content = m.Content,
                    timestamp = m.Timestamp
                })
            });
        }



        #endregion

    }



}
