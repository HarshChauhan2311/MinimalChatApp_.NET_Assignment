using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.ChatApp.Core.Interfaces;
using MinimalChatApp.ChatApp.Shared.DTOs;

namespace MinimalChatApp.ChatApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid message data." });

            var senderIdClaim = User.FindFirst("userId");
            if (senderIdClaim == null || !int.TryParse(senderIdClaim.Value, out int senderId))
                return Unauthorized(new { error = "Unauthorized access." });

            var message = await _messageRepository.SendMessageAsync(senderId, request.ReceiverId, request.Content);

            return Ok(new
            {
                messageId = message.Id,
                senderId = message.SenderId,
                receiverId = message.ReceiverId,
                content = message.Content,
                timestamp = message.Timestamp
            });
        }
    }
}
