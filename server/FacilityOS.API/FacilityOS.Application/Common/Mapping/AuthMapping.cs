using FacilityOS.Application.DTOs.Auth;
using FacilityOS.Domain.Models;

namespace FacilityOS.Application.Common.Mapping;

public static class AuthMapping
{
    public static AuthResult ToAuthResult(this User user, string accessToken, string refreshToken)
    {
        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToResponse()
        };
    }

    public static LoginResponse ToLoginResponse(this AuthResult authResult)
    {
        return new LoginResponse
        {
            AccessToken = authResult.AccessToken,
            User = authResult.User
        };
    }
}