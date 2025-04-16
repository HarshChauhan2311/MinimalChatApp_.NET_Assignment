using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DTO;


namespace MinimalChatApp.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    { 
        #region Private Variables
        private readonly IGroupService _groupService;
        #endregion

        #region Constructors 
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        #endregion

        #region Public methods
        [HttpGet]
        public async Task<IActionResult> GetAllGroupsAsync()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var groups = await _groupService.GetAllGroupsAsync(userId);
            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync([FromBody] CreateGroupRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid request." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var response = await _groupService.CreateGroupAsync(request.Name, userId);

            if (!response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Error) && response.Error.Contains("exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { error = response.Error });

                return StatusCode(response.StatusCode, new { error = response.Error });
            }

            return Ok(new GroupResponseDTO
            {
                Id = response.Data!.Id,
                Name = response.Data.Name,
                CreatedBy   = response.Data.CreatedBy
                
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGroupNameAsync([FromBody] UpdateGroupRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupId <= 0)
                return BadRequest(new { error = "Invalid group update request." });

            var response = await _groupService.UpdateGroupNameAsync(request.GroupId, request.Name);
            if (!response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Error) && response.Error.Contains("exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { error = response.Error });

                return StatusCode(response.StatusCode, new { error = response.Error });
            }

            return Ok(new GroupResponseDTO
            {
                Id = response.Data!.Id,
                Name = response.Data.Name,

            });
         
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGroupAsync([FromBody] DeleteGroupRequestDTO request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!ModelState.IsValid || request.GroupId <= 0 || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest(new { error = "Invalid group deletion request." });

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized(new { error = "Unauthorized access." });

            var response = await _groupService.DeleteGroupAsync(request.GroupId, request.Name, currentUserId, currentUserId);
            if (!response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Error) && response.Error.Contains("exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { error = response.Error });

                return StatusCode(response.StatusCode, new { error = response.Error });
            }


            return Ok(new GroupResponseDTO
            {
                Id = response.Data!.Id,
                Name = response.Data.Name,
                CreatedBy = response.Data.CreatedBy

            });
        }
        #endregion
    }
}
