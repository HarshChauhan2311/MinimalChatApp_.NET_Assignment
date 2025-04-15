using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MinimalChatApp.Entity;
using MinimalChatApp.DTO;
using AutoMapper;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;
using MinimalChatApp.BAL.IServices;
using MinimalChatApp.DAL.IRepositories;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace MinimalChatApp.BAL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;  // Use IUserRepository instead of Generic Repository
        private readonly JwtTokenService _jwtService;
        private readonly IErrorLogService _errorLogService;
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthService(IUserRepository userRepo, JwtTokenService jwtService, IErrorLogService errorLogService, UserManager<ApplicationUser> userManager)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _errorLogService = errorLogService;
            _userManager = userManager;
        }

        public async Task<(bool IsSuccess, int StatusCode, string Error, string Token, ApplicationUser? User)> LoginAsync(LoginRequestDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return (false, 400, "Email and password are required.", string.Empty, null);

            var user = await _userManager.FindByNameAsync(login.Email);
            if (user == null)
            {
                return (false, 404, "User not found.", string.Empty, null);
            }

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                try
                {
                    var token = _jwtService.GenerateToken(user);
                    return (true, 200, string.Empty, token, user);
                }
                catch (Exception ex)
                {
                    await _errorLogService.LogAsync(ex);
                    return (false, 500, "An error occurred while generating token.", string.Empty, null);
                }
            }
            else
            {
                return (false, 401, "Invalid credentials.", string.Empty, null);
            }
        }


        public async Task<(bool IsSuccess, int StatusCode, string Error, ApplicationUser? User)> RegisterAsync(RegisterRequestDTO requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Email) || string.IsNullOrWhiteSpace(requestDto.Name) || string.IsNullOrWhiteSpace(requestDto.Password))
            {
                return (false, 400, "Registration failed due to validation errors.", null);
            }

            var existingUser = await _userRepo.GetByEmailAsync(requestDto.Email);
            if (existingUser != null)
            {
                return (false, 409, "Email already registered.", null);
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = requestDto.Email,
                    Email = requestDto.Email,
                    Name = requestDto.Name,
                };

                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, requestDto.Password);


                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var error = string.Join("; ", result.Errors.Select(e => e.Description));
                    return (false, 400, error, null);
                }

                return (true, 200, string.Empty, user);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return (false, 500, ex.Message, null);
            }
        }


        public async Task<IActionResult> GetUsersAsync(ClaimsPrincipal user)
        {
            try
            {
                var currentUserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserEmail))
                    return new UnauthorizedObjectResult(new { error = "Unauthorized access" });

                var users = await _userRepo.GetAllAsync();  // Using IUserRepository
                                                            //var result = users.Select(u => new { id = u.Id, name = u.Name, email = u.Email });
                                                            // Exclude the current user from the list
                var filteredUsers = users
                    .Where(u => !u.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
                    .Select(u => new { id = u.Id, name = u.Name, email = u.Email });
                return new OkObjectResult(new { users = filteredUsers });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new BadRequestObjectResult(ex);
            }
        }

    }
}
