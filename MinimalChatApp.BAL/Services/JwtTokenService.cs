using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Entity;
using MinimalChatApp.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MinimalChatApp.BAL.Services
{
    public class JwtTokenService
    {
        #region Private Variables
        private readonly IConfiguration _config;
        #endregion

        #region Constructors 
        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region Public Method
        public string GenerateToken(User user)
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key) || key.Length < 16)
            {
                throw new Exception("JWT Key must be at least 16 characters long.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),                 
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
