using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.DTOs;
using System.Security.Claims;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<IActionResult> LoginAsync(LoginRequest login);
        Task<IActionResult> RegisterAsync(RegisterRequest request);
        Task<IActionResult> GetUsersAsync(ClaimsPrincipal user);
    }
}
