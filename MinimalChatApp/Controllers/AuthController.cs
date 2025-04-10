using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using MinimalChatApp.Services;
using MinimalChatApp.DTOs;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Private Variables
        private readonly IAuthService _authService;

        #endregion

        #region Constructors 
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Public methods
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest login)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(login.Email) ||
                string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new { error = "Email and password are required." });
            }

            return await _authService.LoginAsync(login);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { error = "Registration failed due to validation errors." });
            }

            return await _authService.RegisterAsync(request);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            return await _authService.GetUsersAsync(User);
        }

        #endregion

    }
}
