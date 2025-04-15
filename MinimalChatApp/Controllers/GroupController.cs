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
    [Route("api")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("group")]
        public async Task<IActionResult> CreateGroupAsync([FromBody] CreateGroupRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid request." });

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, group) = await _groupService.CreateGroupAsync(request.Name, userId);

            if (!isSuccess)
            {
                if (error?.Contains("exists") == true)
                    return Conflict(new { error });

                return BadRequest(new { error });
            }

            return Ok(new
            {
                groupId = group!.GroupId,
                name = group.GroupName,
                createdBy = group.CreatedBy
            });
        }

        [HttpPut("group")]
        public async Task<IActionResult> UpdateGroupNameAsync([FromBody] UpdateGroupRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupId <= 0)
                return BadRequest(new { error = "Invalid group update request." });

            var (isSuccess, error, group) = await _groupService.UpdateGroupNameAsync(request.GroupId, request.Name);

            if (!isSuccess)
            {
                if (error?.Contains("exists") == true)
                    return Conflict(new { error });

                return BadRequest(new { error });
            }

            return Ok(new
            {
                groupId = group!.GroupId,
                name = group.GroupName
            });
        }

        [HttpDelete("group")]
        public async Task<IActionResult> DeleteGroupAsync([FromBody] DeleteGroupRequestDTO request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!ModelState.IsValid || request.GroupId <= 0 || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest(new { error = "Invalid group deletion request." });

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized(new { error = "Unauthorized access." });

            var (isSuccess, error, group) = await _groupService.DeleteGroupAsync(request.GroupId, request.Name, currentUserId, currentUserId);

            if (!isSuccess)
            {
                if (error?.Contains("already deleted") == true)
                    return Conflict(new { error });

                return BadRequest(new { error });
            }

            return Ok(new
            {
                groupId = group!.GroupId,
                name = group.GroupName,
                email = group.Creator.Email
            });
        }


        [HttpPost("member")]
        public async Task<IActionResult> AddMemberAsync([FromBody] AddMemberRequestDTO request)
        {
            if (!ModelState.IsValid || request.UserId <= 0 || request.GroupId <= 0)
                return BadRequest(new { error = "Invalid user or group ID." });

            var (isSuccess, error, member) = await _groupService.AddMemberAsync(request.UserId, request.GroupId);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new
            {
                id = member!.Id,
                userId = member.UserId,
                groupId = member.GroupId
            });
        }

        [HttpPut("access")]
        public async Task<IActionResult> UpdateMemberAccessAsync([FromBody] UpdateMemberAccessRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupMemberId <= 0)
                return BadRequest(new { error = "Invalid request parameters." });

            var (isSuccess, error, member) = await _groupService.UpdateMemberAccessAsync(
                request.GroupMemberId, request.AccessType, request.Days);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new
            {
                id = member!.Id,
                userId = member.UserId,
                groupId = member.GroupId,
                accessType = member.AccessType,
                days = member.Days
            });
        }

        [HttpDelete("member")]
        public async Task<IActionResult> RemoveMemberAsync([FromQuery] int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Invalid group member ID." });

            var (isSuccess, error) = await _groupService.RemoveMemberAsync(id);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new { message = "Member deleted successfully." });
        }


        [HttpGet("groups")]
        public async Task<IActionResult> GetAllGroupsAsync()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var groups = await _groupService.GetAllGroupsAsync(userId);
            return Ok(groups);
        }


    }
}
