using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MinimalChatApp.Interfaces.IServices;

namespace MinimalChatApp.MinimalChatApp.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;
        private readonly IErrorLogService _errorLogService;

        public GoogleAuthService(IUserRepository userRepo, JwtTokenService jwtService, IErrorLogService errorLogService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _errorLogService = errorLogService;
        }
        public IActionResult GoogleLogin(ControllerBase controller)
        {
            try
            {
                var redirectUrl = controller.Url.Action("GoogleResponse", "SocialLogin");
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return controller.Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }

        public async Task<string> HandleGoogleLoginAsync(HttpContext httpContext)
        {
            try
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
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
            }
        }
    }
}
