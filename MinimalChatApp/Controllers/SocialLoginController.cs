using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.BAL.IServices;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class SocialLoginController : ControllerBase
    { 
        #region Private Variables
        private readonly IGoogleAuthService _googleAuthService;
        #endregion

        #region Constructors 
        public SocialLoginController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }
        #endregion

        #region Public methods
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "SocialLogin");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var queryString = await _googleAuthService.HandleGoogleLoginAsync(HttpContext);

            // Check if it is an error
            if (queryString.StartsWith("error"))
                return Redirect($"http://localhost:4200/login?{queryString}");

            return Redirect($"http://localhost:4200/chat?{queryString}");
        }
        #endregion
    }
}
