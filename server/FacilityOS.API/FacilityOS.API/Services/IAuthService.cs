using FacilityOS.API.Models;

namespace FacilityOS.API.Services
{
    public interface IAuthService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();

        bool ValidateToken(string token);
        bool IsTokenExpired(string token);

        string? GetUserIdFromToken(string token);
        string? GetUserNameFromToken(string token);
        string? GetUserEmailFromToken(string token);
    }
}