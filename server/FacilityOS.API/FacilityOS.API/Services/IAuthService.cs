using System.Security.Claims;
using FacilityOS.API.Models;

namespace FacilityOS.API.Services;

public interface IAuthService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    //Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token);
    //bool IsTokenExpired(string token);
}