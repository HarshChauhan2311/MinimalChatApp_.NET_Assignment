using Microsoft.AspNetCore.Mvc;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    /// <summary>
    /// Service for handling Google authentication.
    /// </summary>
    public interface IGoogleAuthService
    {
        /// <summary>
        /// Initiates the Google login process and redirects to the Google OAuth endpoint.
        /// </summary>
        /// <param name="controller">The controller base to generate the challenge result.</param>
        /// <returns>An <see cref="IActionResult"/> to redirect to Google login.</returns>
        IActionResult GoogleLogin(ControllerBase controller);

        /// <summary>
        /// Handles the callback after Google login is completed and retrieves the access token or user info.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the current request.</param>
        /// <returns>A task representing the access token or user info as a string.</returns>
        Task<string> HandleGoogleLoginAsync(HttpContext httpContext);
    }

}
