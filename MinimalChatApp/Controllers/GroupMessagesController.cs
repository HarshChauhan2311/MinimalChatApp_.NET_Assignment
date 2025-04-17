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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> SendGroupMessage([FromForm] SendGroupMessageRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupId <= 0 || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Invalid request data." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int senderId))
                return Unauthorized(new { error = "Unauthorized access." });

            string? fileUrl = null;
            string? contentType = null;

            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.Attachment.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (request.Attachment != null)
            {

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Attachment.CopyToAsync(stream);
                }

                fileUrl = $"/uploads/{fileName}";
                contentType = request.Attachment.ContentType;
            }

            var response = await _messageService.SendGroupMessageAsync(
                senderId, request.GroupId, request.Content, fileUrl, contentType);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            // If file exists, return it as a download
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var downloadName = Path.GetFileName(filePath);
                return File(fileBytes, contentType, downloadName);
            }

            return Ok("Message sent but no file to download.");
        }



        [HttpGet]
        public async Task<IActionResult> GetGroupConversationHistory([FromBody] GroupChatHistoryRequestDTO request)
        {
            // Get userId from JWT token
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            // Validate inputs
            if (request.Count <= 0 ||
                string.IsNullOrWhiteSpace(request.Sort) ||
                !(request.Sort.Equals("asc", StringComparison.OrdinalIgnoreCase) || request.Sort.Equals("desc", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { error = "Invalid request parameters." });
            }

            var beforeDate = request.Before ?? DateTime.UtcNow;

            // Call service
            var response = await _messageService.GetGroupMessagesAsync(
                userId, request.GroupId, beforeDate, request.Count, request.Sort);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            return Ok(new
            {
                messages = response.Data!.Select(m => new
                {
                    id = m.MessageId,
                    senderId = m.SenderId,
                    content = m.Content,
                    timestamp = m.Timestamp
                })
            });
        }


        #endregion
    }
}
