using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.Entity;
using MinimalChatApp.DTO;
using AutoMapper;
using MinimalChatApp.DAL.IRepositories;

namespace MinimalChatApp.DAL.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        #region Private Variables
        private readonly AppDbContext _context;

        //private readonly IErrorLogService _errorLogService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors 

        public UserRepository(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion

        #region Public methods
        public async Task<UserDTO?> GetByIdAsync(int userId)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            //return user != null ? _mapper.Map<UserDTO>(user) : null;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null ? _mapper.Map<UserDTO>(user) : null;


        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);


        }

        public async Task<bool> AddUserAsync(ApplicationUser user)
        {

            await _context.Users.AddAsync(user); // Asynchronous add
            return await _context.SaveChangesAsync() > 0;

        }
        public async Task<List<UserDTO>> GetOtherUsersAsync(string currentUserEmail)
        {
            return await _context.Users
                .Where(u => !u.Email.ToLower().Equals(currentUserEmail.ToLower()))
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();
        }

        public async Task<UserDTO> GetUserDetailByEmailAsync(string email)
        {
            return await _context.Users
                 .Where(u => u.Email.ToLower() == email.ToLower())
                 .Select(u => new UserDTO
                 {
                     Id = u.Id,
                     Name = u.Name,
                     Email = u.Email
                 })
                 .FirstOrDefaultAsync();

        }

        #endregion
    }
}
