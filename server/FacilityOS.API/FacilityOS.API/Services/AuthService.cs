using FacilityOS.API.Models;
using FacilityOS.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FacilityOS.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly int _bcryptWorkFactor;

        public AuthService(IOptions<JwtSettings> jwtSettings, IConfiguration config)
        {
            _jwtSettings = jwtSettings.Value;
            _bcryptWorkFactor = config.GetValue<int>("BCrypt:WorkFactor", 12);
        }

        public string GenerateAccessToken(User user)
        {
            var key = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new("entity_type", user.EntityType.ToString()),
            };

            if (user.EntityId.HasValue)
                claims.Add(new Claim("entity_id", user.EntityId.Value.ToString()));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
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
            var key = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
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
