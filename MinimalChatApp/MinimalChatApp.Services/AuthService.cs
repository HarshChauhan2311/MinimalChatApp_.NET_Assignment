using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Models;
using MinimalChatApp.Services;
using MinimalChatApp.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;

namespace MinimalChatApp.MinimalChatApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;
        private readonly IErrorLogService _errorLogService;


        public AuthService(IUserRepository userRepo, JwtTokenService jwtService, IErrorLogService errorLogService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _errorLogService = errorLogService;
        }

        public async Task<IActionResult> LoginAsync(LoginRequest login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return new BadRequestObjectResult(new { error = "Email and password are required." });

            var user = await _userRepo.GetByEmailAsync(login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return new UnauthorizedObjectResult(new { error = "Invalid credentials." });

            try
            {
                var token = _jwtService.GenerateToken(user);
                return new OkObjectResult(new
                {
                    token,
                    profile = new { id = user.Id, name = user.Name, email = user.Email }
                });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }

        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
                return new BadRequestObjectResult(new { error = "Registration failed due to validation errors." });

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return new ConflictObjectResult(new { error = "Email already registered." });

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
                    return new BadRequestObjectResult(new { error = "Server error during registration." });

                return new OkObjectResult(new { userId = user.Id, name = user.Name, email = user.Email });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }


        public async Task<IActionResult> GetUsersAsync(ClaimsPrincipal user)
        {
            try
            {
                var currentUserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserEmail))
                    return new UnauthorizedObjectResult(new { error = "Unauthorized access" });

                var users = await _userRepo.GetOtherUsersAsync(currentUserEmail);
                var result = users.Select(u => new { id = u.Id, name = u.Name, email = u.Email });
                return new OkObjectResult(new { users = result });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }

    }
}
