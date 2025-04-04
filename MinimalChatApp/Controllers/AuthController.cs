using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using MinimalChatApp.Services;
using MinimalChatApp.Interfaces;
using MinimalChatApp.DTOs;
using MinimalChatApp.Interfaces;


namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Private Variables
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;
        private readonly IErrorLogService _errorLogService;

        #endregion

        #region Constructors 
        public AuthController(IUserRepository userRepo, JwtTokenService jwtService, IErrorLogService errorLogService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _errorLogService = errorLogService;
        }
        #endregion

        #region Public methods
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            // Check for null or empty fields
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(login.Email) ||
                string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new
                {
                    error = "Email and password are required."
                });
            }

            // Retrieve user by email
            var user = await _userRepo.GetByEmailAsync(login.Email);
            // Validate credentials
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized(new
                {
                    error = "Invalid credentials."
                });
            }
            try
            {
                // Generate JWT token
                var token = _jwtService.GenerateToken(user);

                // Return 200 OK with token and profile
                return Ok(new
                {
                    token,
                    profile = new
                    {
                        id = user.Id,
                        name = user.Name,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Validate request body
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    error = "Registration failed due to validation errors."
                });
            }

            // Check for existing email
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Conflict(new
                {
                    error = "Registration failed because the email is already registered."
                });
            }

            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    Email = request.Email,
                    Name = request.Name,
                    PasswordHash = hashedPassword
                };

                var result = await _userRepo.AddUserAsync(user);

                if (!result)
                {
                    return BadRequest(new
                    {
                        error = "Registration failed due to a server error."
                    });
                }

                return Ok(new
                {
                    userId = user.Id,
                    name = user.Name,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {

            string? currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            //var currentUserEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return Unauthorized(new { error = "Unauthorized access" });
            }

            var users = await _userRepo.GetOtherUsersAsync(currentUserEmail);

            if (users == null || !users.Any())
            {
                return Ok(new { users = Array.Empty<object>() });
            }
            try
            {
                var result = users.Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email
                });

                return Ok(new { users = result });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return BadRequest(ex);
            }
        }
        #endregion

    }
}
