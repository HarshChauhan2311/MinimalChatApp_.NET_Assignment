using MinimalChatApp.ChatApp.Shared.DTOs;
using MinimalChatApp.ChatApp.Shared.Models;

namespace MinimalChatApp.ChatApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="request">Registration data including email, name, and password.</param>
        /// <returns>
        /// Tuple of:
        /// - bool: Success status
        /// - string: Error message if any
        /// - User: Registered user object (null if failed)
        /// </returns>
        Task<(bool Success, string? Error, User? RegisteredUser)> RegisterAsync(RegisterRequest request);

        Task<List<User>> GetOtherUsersAsync(string currentUserEmail);
    }
}
