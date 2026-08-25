using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FacilityOS.API.Common;
using FacilityOS.API.Models;
using FacilityOS.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens; 

namespace FacilityOS.API.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly JsonWebTokenHandler _tokenHandler; 

    public AuthService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _tokenHandler = new JsonWebTokenHandler();
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, user.Id.ToString() },
            { JwtRegisteredClaimNames.Name, user.Name },
            { JwtRegisteredClaimNames.Email, user.Email },
            { ClaimTypes.Role, user.Role },
            { AppConstants.Claims.EntityType, user.EntityType.ToString() }
        };

        if (user.EntityId.HasValue)
        {
            claims.Add(AppConstants.Claims.EntityId, user.EntityId.Value.ToString());
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = credentials
        };

        return _tokenHandler.CreateToken(descriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    //public async Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token)
    //{
    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

    //    try
    //    {
    //        var validationResult = await _tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = false,
    //            ValidateIssuerSigningKey = true,
    //            ValidIssuer = _jwtSettings.Issuer,
    //            ValidAudience = _jwtSettings.Audience,
    //            IssuerSigningKey = key,
    //            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
    //        });

    //        if (!validationResult.IsValid) return null;

    //        return new ClaimsPrincipal(validationResult.ClaimsIdentity);
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //}

    //public bool IsTokenExpired(string token)
    //{
    //    try
    //    {
    //        var jwt = _tokenHandler.ReadJsonWebToken(token);
    //        return jwt.ValidTo < DateTime.UtcNow;
    //    }
    //    catch
    //    {
    //        return true;
    //    }
    //}
}
