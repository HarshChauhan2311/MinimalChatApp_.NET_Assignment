using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DTOs;
using MinimalChatApp.Data;
using MinimalChatApp.Models;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.MinimalChatApp.DTOs;
using AutoMapper;

namespace MinimalChatApp.MinimalChatApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;
        private readonly IErrorLogService _errorLogService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors 
        public UserRepository(AppDbContext context, IErrorLogService errorLogService, IMapper mapper)
        {
            _context = context;
            _errorLogService = errorLogService;
            _mapper = mapper;
        }
        #endregion

        #region Public methods
        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user != null ? _mapper.Map<UserDto>(user) : null;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return null;
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
                await _context.Users.AddAsync(user); // Asynchronous add
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return false; // Let the controller handle the 500 response
            }
        }
        public async Task<List<UserDto>> GetOtherUsersAsync(string currentUserEmail)
        {
            try
            {
                return await _context.Users
                    .Where(u => !u.Email.ToLower().Equals(currentUserEmail.ToLower()))
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new List<UserDto>();
            }
        }


        #endregion
    }
}
