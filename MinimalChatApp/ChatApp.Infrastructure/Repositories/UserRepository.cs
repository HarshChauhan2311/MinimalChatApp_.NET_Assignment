using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.ChatApp.Core.Interfaces;
using MinimalChatApp.ChatApp.Infrastructure.Data;
using MinimalChatApp.ChatApp.Shared.DTOs;
using MinimalChatApp.ChatApp.Shared.Models;

namespace MinimalChatApp.ChatApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch
            {
                return null;
            }

        }


        public async Task<(bool Success, string? Error, User? RegisteredUser)> RegisterAsync(RegisterRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return (false, "All fields are required.", null);
            }

            if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                return (false, "Invalid email format.", null);
            }

            // ✅ Use async EF method to check for existing user
            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return (false, "Email is already registered.", null);
            }

            // ✅ Use async SaveChanges
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return (true, null, user);
        }
        public async Task<List<User>> GetOtherUsersAsync(string currentUserEmail)
        {
            return await _context.Users
                .Where(u => u.Email != currentUserEmail)
                .ToListAsync();
        }
    }
}
