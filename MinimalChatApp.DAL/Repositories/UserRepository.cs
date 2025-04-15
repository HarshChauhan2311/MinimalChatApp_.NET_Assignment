using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.Entity;
using MinimalChatApp.DTO;
using AutoMapper;
using MinimalChatApp.DAL.IRepositories;

namespace MinimalChatApp.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;

        //private readonly IErrorLogService _errorLogService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors 

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        #endregion

        #region Public methods
        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null ? _mapper.Map<UserDto>(user) : null;

        }

        public async Task<User?> GetByEmailAsync(string email)
        {

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);


        }

        public async Task<bool> AddUserAsync(User user)
        {

            await _context.Users.AddAsync(user); // Asynchronous add
            return await _context.SaveChangesAsync() > 0;

        }
        public async Task<List<UserDto>> GetOtherUsersAsync(string currentUserEmail)
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

        #endregion
    }
}
