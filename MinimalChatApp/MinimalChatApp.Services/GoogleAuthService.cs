using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MinimalChatApp.MinimalChatApp.Services
{
    public class GoogleAuthService: IGoogleAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;

        public GoogleAuthService(IUserRepository userRepo, JwtTokenService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }
        public IActionResult GoogleLogin(ControllerBase controller)
        {
            var redirectUrl = controller.Url.Action("GoogleResponse", "SocialLogin");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return controller.Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<string> HandleGoogleLoginAsync(HttpContext httpContext)
        {
            var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
                return "error=GoogleAuthFailed";

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return "error=EmailNotFound";

            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return "error=UserNotRegistered";

            var token = _jwtService.GenerateToken(user);

            return $"token={token}&id={(user.Id)}&name={Uri.EscapeDataString(user.Name)}&email={Uri.EscapeDataString(user.Email)}";
        }
    }
}
