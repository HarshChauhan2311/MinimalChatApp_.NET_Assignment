using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;
using System.Security.Claims;

namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Defines authentication and user-related operations such as login, registration, and user retrieval.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using the provided login credentials.
        /// </summary>
        /// <param name="login">Login details including username/email and password.</param>
        /// <returns>A tuple indicating success status, status code, error message, JWT token, and user details.</returns>
        Task<ServiceResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO login);

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="request">Registration details for the new user.</param>
        /// <returns>A tuple indicating success status, status code, error message, and user information.</returns>
        Task<ServiceResponseDTO<ApplicationUser>> RegisterAsync(RegisterRequestDTO request);

        /// <summary>
        /// Retrieves a list of users based on the authenticated user's permissions.
        /// </summary>
        /// <param name="user">The current authenticated user's claims principal.</param>
        /// <returns>An <see cref="IActionResult"/> containing the user list or an error response.</returns>
        Task<ServiceResponseDTO<List<UserDTO>>> GetUsersAsync(ClaimsPrincipal user);

        /// <summary>
        /// Retrieves a Detail of user based on the authenticated user's permissions.
        /// </summary>
        /// <param name="user">The current authenticated user's claims principal.</param>
        /// <returns>An <see cref="IActionResult"/> containing the user list or an error response.</returns>
        Task<ServiceResponseDTO<UserDTO>> GetUserDetailAsync(string email);
    }

}

