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

        public async Task<ServiceResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return new ServiceResponseDTO<LoginResponseDTO> { IsSuccess = false, StatusCode = 400, Error = "Email and password are required." };

            var user = await _userManager.FindByNameAsync(login.Email);
            if (user == null)
            {
                return new ServiceResponseDTO<LoginResponseDTO> { IsSuccess = false, StatusCode = 404, Error = "User not found." };
            }

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                try
                {
                    var token = _jwtService.GenerateToken(user);
                    return new ServiceResponseDTO<LoginResponseDTO>
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Data = new LoginResponseDTO
                        {
                            Token = token,
                            User = user
                        }
                    };
                }
                catch (Exception ex)
                {
                    await _errorLogService.LogAsync(ex);
                    return new ServiceResponseDTO<LoginResponseDTO> { IsSuccess = false, StatusCode = 500, Error = "An error occurred while generating token." };
                }
            }
            else
            {
                return new ServiceResponseDTO<LoginResponseDTO> { IsSuccess = false, StatusCode = 401, Error = "Invalid credentials." };
            }
        }


        public async Task<ServiceResponseDTO<ApplicationUser>> RegisterAsync(RegisterRequestDTO requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Email) || string.IsNullOrWhiteSpace(requestDto.Name) || string.IsNullOrWhiteSpace(requestDto.Password))
            {
                return new ServiceResponseDTO<ApplicationUser> { IsSuccess = false, StatusCode = 400, Error = "Registration failed due to validation errors." };
            }

            var existingUser = await _userRepo.GetByEmailAsync(requestDto.Email);
            if (existingUser != null)
            {
                return new ServiceResponseDTO<ApplicationUser> { IsSuccess = false, StatusCode = 409, Error = "Email already registered." };
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
                    return new ServiceResponseDTO<ApplicationUser> { IsSuccess = false, StatusCode = 400, Error = error };
                }

                return new ServiceResponseDTO<ApplicationUser> { IsSuccess = true, StatusCode = 200, Data = user };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<ApplicationUser> { IsSuccess = false, StatusCode = 500, Error = "Internal server error." };
            }
        }


        public async Task<ServiceResponseDTO<List<UserDTO>>> GetUsersAsync(ClaimsPrincipal user)
        {
            try
            {
                var currentUserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserEmail))
                    return new ServiceResponseDTO<List<UserDTO>>
                    {
                        IsSuccess = false,
                        StatusCode = 401,
                        Error = "Unauthorized access."
                    };

                var users = await _userRepo.GetAllAsync();  // Using IUserRepository
                                                            //var result = users.Select(u => new { id = u.Id, name = u.Name, email = u.Email });
                                                            // Exclude the current user from the list
                                                            // Exclude the current user from the list and map to UserDTO
                var filteredUsers = users
                    .Where(u => !u.Email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email
                    }).ToList();

                return new ServiceResponseDTO<List<UserDTO>>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = filteredUsers
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<List<UserDTO>>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Error = "Failed to retrieve user list."
                };
            }
        }

        public async Task<ServiceResponseDTO<UserDTO>> GetUserDetailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ServiceResponseDTO<UserDTO>
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Error = "Unauthorized access."
                };
            }

            try
            {
                var user = await _userRepo.GetUserDetailByEmailAsync(email);
                
                return new ServiceResponseDTO<UserDTO>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                await _errorLogService.LogAsync(ex);
                return new ServiceResponseDTO<UserDTO>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Error = "Failed to retrieve user list."
                };
            }
        }


    }
}
