using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.DTO;
using System.Security.Claims;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Provides authentication and user management services such as login, registration, and retrieving users.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user based on login credentials.
        /// </summary>
        /// <param name="login">The login request containing username/email and password.</param>
        /// <returns>An <see cref="IActionResult"/> containing the authentication result, such as a JWT token or error.</returns>
        Task<IActionResult> LoginAsync(LoginRequestDTO login);

        /// <summary>
        /// Registers a new user with the provided registration information.
        /// </summary>
        /// <param name="request">The registration request containing user details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success or failure of registration.</returns>
        Task<IActionResult> RegisterAsync(RegisterRequestDTO request);

        /// <summary>
        /// Retrieves a list of users based on the current authenticated user's context.
        /// </summary>
        /// <param name="user">The current authenticated user principal.</param>
        /// <returns>An <see cref="IActionResult"/> containing the list of users or access denial information.</returns>
        Task<IActionResult> GetUsersAsync(ClaimsPrincipal user);
    }

}
