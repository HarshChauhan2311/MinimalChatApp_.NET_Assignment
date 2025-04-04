using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.DTOs;
using MinimalChatApp.Interfaces;
using MinimalChatApp.Interfaces;

namespace MinimalChatApp.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        #region Private Variables
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IErrorLogService errorLogService)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "Invalid message data." }); // 400 Bad Request
            }

            var senderIdClaim = User.FindFirst("userId");
            if (senderIdClaim == null || !int.TryParse(senderIdClaim.Value, out int senderId))
            {
                return Unauthorized(new { error = "Unauthorized access." }); // 401 Unauthorized
            }

            // Optional: Verify receiver exists
            var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
                return NotFound(new { error = "Receiver not found." });
            try
            {
                var message = await _messageRepository.SendMessageAsync(senderId, request.ReceiverId, request.Content);
                if (message == null)
                    return BadRequest(new { error = "- Message sending failed due to validation errors" }); // Optional safeguard
                return Ok(new
                {
                    messageId = message.Id,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    content = message.Content,
                    timestamp = message.Timestamp
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }

        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] EditMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Message content cannot be empty." });

            // Get user ID from token
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "Unauthorized access." }); // 401

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                return NotFound(new { error = "Message not found." });

            if (message.SenderId != userId)
                return Unauthorized(new { error = "Unauthorized access" });
            try
            {
                message.Content = request.Content;
                await _messageRepository.UpdateAsync(message);

                return Ok(new
                {
                    messageId = message.Id,
                    content = message.Content,
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    timestamp = message.Timestamp
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }

        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "Unauthorized access." }); // 401

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                return NotFound(new { error = "Message not found." });

            if (message.SenderId != userId)
                return Unauthorized(new { error = "Unauthorized access." });

            try
            {
                await _messageRepository.DeleteAsync(message);

                return Ok(new { message = "Message deleted successfully." });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetConversationHistory(
        [FromQuery] int userId,
        [FromQuery] DateTime? before,
        [FromQuery] int count = 20,
        [FromQuery] string sort = "asc")
        {
            // Validate user claim
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
                return Unauthorized(new { error = "Unauthorized access." }); // 401
                                                                             // Prevent self-conversation

            if (count <= 0 || sort.ToLower() != "asc" && sort.ToLower() != "desc")
                return BadRequest(new { error = "Invalid request parameters." }); // 400

            var timestamp = before ?? DateTime.UtcNow;

            try
            {
                // Fetch conversation
                var messages = await _messageRepository.GetConversationAsync(currentUserId, userId, timestamp, count, sort.ToLower());

                if (messages == null || !messages.Any())
                    return NotFound(new { error = "No conversation history found." }); // 404

                return Ok(new
                {
                    messages = messages.Select(m => new
                    {
                        id = m.Id,
                        senderId = m.SenderId,
                        receiverId = m.ReceiverId,
                        content = m.Content,
                        timestamp = m.Timestamp
                    })
                }); // 200 OK
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
