using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;

namespace MinimalChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Private Variables
        private readonly IAuthService _authService;

        #endregion

        #region Constructors 
        public UserController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Public methods
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await _authService.GetUsersAsync(User);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { error = response.Error });

            return Ok(new
            {
                users = response.Data.Select(user => new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                })
            });
        }

        #endregion
    }
}
