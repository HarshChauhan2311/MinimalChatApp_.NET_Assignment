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
            return await _authService.GetUsersAsync(User);
        }
        #endregion
    }
}
