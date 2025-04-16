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
            // Extract user ID from claims
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            // Validate the input parameters
            if (groupId <= 0 || string.IsNullOrWhiteSpace(message))
                return BadRequest(new { error = "Invalid request parameters." });

            // Call the service to search group messages
            var response = await _messageService.SearchGroupMessagesAsync(userId, groupId, message);

            // If the service returns failure, return the appropriate status code and error
            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            // Format the response to match the expected output
            var messages = response.Data;
            var result = messages.Select(m => new
            {
                id = m.GroupId,
                messageId = m.MessageId,
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                content = m.Content,
                timestamp = m.Timestamp
            }).ToList();

            return Ok(new
            {
                messages = result
            });
        }

        #endregion
    }
}
