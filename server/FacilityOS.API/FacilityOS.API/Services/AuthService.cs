using FacilityOS.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FacilityOS.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials = new SigningCredentials(Key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public bool ValidateToken(string token)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsTokenExpired(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }

        public string? GetUserIdFromToken(string token)
            => ReadClaim(token, ClaimTypes.NameIdentifier);

        public string? GetUserNameFromToken(string token)
            => ReadClaim(token, ClaimTypes.Name);

        public string? GetUserEmailFromToken(string token)
            => ReadClaim(token, ClaimTypes.Email);

        private static string? ReadClaim(string token, string claimType)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            }
            catch
            {
                return null;
            }
        }

    }
}
