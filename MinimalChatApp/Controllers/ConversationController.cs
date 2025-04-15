using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.Hubs;

namespace MinimalChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        #region Private Variables
        private readonly IMessageService _messageService;
        #endregion

        #region Constructors 
        public ConversationController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        #endregion

        #region Public methods

        [HttpGet("Search")]
        public async Task<IActionResult> GetConversationHistoryAsync(
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
