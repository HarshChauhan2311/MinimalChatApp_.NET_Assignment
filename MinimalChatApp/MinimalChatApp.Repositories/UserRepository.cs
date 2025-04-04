using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DTOs;
using MinimalChatApp.Data;
using MinimalChatApp.Interfaces;
using MinimalChatApp.Models;
using MinimalChatApp.Interfaces;

namespace MinimalChatApp.MinimalChatApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;
        #endregion

        #region Constructors 
        public UserRepository(AppDbContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        public async Task<User?> GetByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null; // Let the controller handle the 500 response
            }
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null; // Let the controller handle the 500 response
            }

        }
        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false; // Let the controller handle the 500 response
            }
        }
        public async Task<List<User>> GetOtherUsersAsync(string currentUserEmail)
        {
            try
            {
                return await _context.Users
                    .Where(u => u.Email != currentUserEmail)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null; // Let the controller handle the 500 response
            }
        }
        #endregion
    }
}
