using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinimalChatApp.DTO;
using MinimalChatApp.BAL.IServices;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MinimalChatApp.Entity;
using Newtonsoft.Json.Linq;


namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Private Variables
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        #endregion

        #region Constructors 
        public AuthController(IAuthService authService,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Public methods
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO login)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(login.Email) ||
                string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new { error = "Email and password are required." });
            }

            var (isSuccess, statusCode, error, token, user) = await _authService.LoginAsync(login);

            // If login failed, return Unauthorized
            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            // Return successful response with the token and user profile
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


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid registration data." });

            var (isSuccess, statusCode, error, user) = await _authService.RegisterAsync(request);

            if (!isSuccess)
                return StatusCode(statusCode, new { error });

            return Ok(new { userId = user.Id, name = user.Name, email = user.Email });
        }

        #endregion

    }
}
