using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.BAL.Services;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

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

            var response = await _memberService.AddMemberAsync(request);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });



            var res = new AddMemberResponseDTO
            {
                Id = response.Data!.Id,
                UserId = response.Data.UserId,
                GroupId = response.Data.GroupId
            };


            return Ok(res);
          
        }

       
        [HttpDelete]
        public async Task<IActionResult> RemoveMemberAsync([FromQuery] int id)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "Unauthorized access." });

            var response = await _memberService.RemoveMemberAsync(id);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            return Ok(response.Data); // contains IsSuccess, Message, StatusCode
        }

        #endregion
    }
}
