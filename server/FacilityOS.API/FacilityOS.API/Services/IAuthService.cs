namespace FacilityOS.API.Services
{
    public interface IAuthService
    {

        string GenerateToken(string userId);

        bool ValidateToken(string token);

        bool IsTokenExpired(string token);
        bool IsTokenRevoked(string token);

        string? GetUserIdFromToken(string token);
        string? GetUserNameFromToken(string token);
        string? GetUserEmailFromToken(string token);
    }
}
