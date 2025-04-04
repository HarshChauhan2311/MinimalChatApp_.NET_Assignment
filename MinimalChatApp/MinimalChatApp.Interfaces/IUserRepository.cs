using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DTOs;
using MinimalChatApp.Models;

namespace MinimalChatApp.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A <see cref="User"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        Task<User?> GetByIdAsync(int userId);

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>
        /// A <see cref="User"/> object if found; otherwise, <c>null</c>.
        /// </returns>
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
        Task<bool> AddUserAsync(User user);

        /// <summary>
        /// Retrieves a list of users excluding the currently logged-in user.
        /// </summary>
        /// <param name="currentUserEmail">The email of the currently logged-in user to exclude from results.</param>
        /// <returns>
        /// A list of <see cref="User"/> objects excluding the specified user.
        /// </returns>
        Task<List<User>> GetOtherUsersAsync(string currentUserEmail);
    }
}
