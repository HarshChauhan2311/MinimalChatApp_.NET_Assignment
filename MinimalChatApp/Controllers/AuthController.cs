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
            // Validate the input
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(login.Email) ||
                string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new ServiceResponseDTO<LoginResponseDTO>
                {
                    IsSuccess = false,
                    Error = "Email and password are required.",
                    StatusCode = 400
                });
            }

            // Call the service to handle login
            var response = await _authService.LoginAsync(login);

            // If login failed, return Unauthorized
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, new ServiceResponseDTO<LoginResponseDTO>
                {
                    IsSuccess = false,
                    Error = response.Error,
                    StatusCode = response.StatusCode
                });
            }

            // Return successful response with the token and user profile
            return Ok(new 
            {
                    Token = response.Data.Token, // Assuming the token is part of response data
                    User = new 
                    {
                        Id = response.Data.User.Id,
                        Name = response.Data.User.Name,
                        Email = response.Data.User.Email
                    }
            });
        }



        [HttpPost("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponseDTO<ApplicationUser>
                {
                    IsSuccess = false,
                    Error = "Invalid registration data.",
                    StatusCode = 400
                });

            var response = await _authService.RegisterAsync(request);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new ServiceResponseDTO<ApplicationUser>
                {
                    IsSuccess = false,
                    Error = response.Error,
                    StatusCode = response.StatusCode
                });

                // If registration is successful, return the user details wrapped in a response
                return Ok(new 
                {
                   Id = response.Data.Id,
                   Name = response.Data.Name,
                   Email = response.Data.Email
                });
        }


        #endregion

    }
}
