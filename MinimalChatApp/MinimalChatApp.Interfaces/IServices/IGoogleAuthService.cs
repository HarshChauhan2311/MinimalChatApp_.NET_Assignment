using Microsoft.AspNetCore.Mvc;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    public interface IGoogleAuthService
    {
        IActionResult GoogleLogin(ControllerBase controller);
        Task<string> HandleGoogleLoginAsync(HttpContext httpContext);
    }
}
