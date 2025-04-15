using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DTO;

namespace MinimalChatApp.API.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {  
        #region Private Variables
        private readonly IMemberService _memberService;
        #endregion

        #region Constructors 
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        #endregion

        #region Public methods

        [HttpPost]
        public async Task<IActionResult> AddMemberAsync([FromBody] AddMemberRequestDTO request)
        {
            if (!ModelState.IsValid || request.UserId <= 0 || request.GroupId <= 0)
                return BadRequest(new { error = "Invalid user or group ID." });

            var (isSuccess, error, member) = await _memberService.AddMemberAsync(request.UserId, request.GroupId);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new
            {
                id = member!.Id,
                userId = member.UserId,
                groupId = member.GroupId
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMemberAccessAsync([FromBody] UpdateMemberAccessRequestDTO request)
        {
            if (!ModelState.IsValid || request.GroupMemberId <= 0)
                return BadRequest(new { error = "Invalid request parameters." });

            var (isSuccess, error, member) = await _memberService.UpdateMemberAccessAsync(
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

        [HttpDelete]
        public async Task<IActionResult> RemoveMemberAsync([FromQuery] int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Invalid group member ID." });

            var (isSuccess, error) = await _memberService.RemoveMemberAsync(id);

            if (!isSuccess)
                return BadRequest(new { error });

            return Ok(new { message = "Member deleted successfully." });
        }
        #endregion
    }
}
