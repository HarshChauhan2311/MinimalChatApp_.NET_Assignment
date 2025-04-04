using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.ChatApp.Core.Interfaces;
using MinimalChatApp.ChatApp.Core.Services;
using MinimalChatApp.ChatApp.Shared.DTOs;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.ChatApp.Shared.Models;
using MinimalChatApp.ChatApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MinimalChatApp.ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;

        public AuthController(IUserRepository userRepo, JwtTokenService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest(new { error = "Email and password are required" });

            var user = await _userRepo.GetByEmailAsync(login.Email);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.Password);
            var g = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }
            else
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new
                {
                    token,
                    profile = new { user.Id, user.Name, user.Email }
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid registration data." });

            var (success, error, registeredUser) = await _userRepo.RegisterAsync(request);

            if (!success)
            {
                if (error == "Email is already registered.")
                    return Conflict(new { error });

                return BadRequest(new { error });
            }

            return Ok(new
            {
                userId = registeredUser!.Id,
                name = registeredUser.Name,
                email = registeredUser.Email
            });
        }

        [Authorize]
        [HttpGet("RetrieveUserList")]
        public async Task<IActionResult> GetUsers()
        {
            var currentUserEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUserEmail))
                return Unauthorized(new { error = "Unauthorized access" });

            var users = await _userRepo.GetOtherUsersAsync(currentUserEmail);

            var result = users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email
            });

            return Ok(new { users = result });
        }

    }
}
