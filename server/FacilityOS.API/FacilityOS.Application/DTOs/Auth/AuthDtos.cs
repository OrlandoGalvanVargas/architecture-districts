using FacilityOS.Application.DTOs.Users;
namespace FacilityOS.Application.DTOs.Auth;

public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record AuthResult
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public UserResponse User { get; init; } = null!;
}

public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public UserResponse User { get; init; } = null!;
}