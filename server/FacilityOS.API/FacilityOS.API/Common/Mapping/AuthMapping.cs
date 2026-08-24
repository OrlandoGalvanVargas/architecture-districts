using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Models;

namespace FacilityOS.API.Common.Mapping;

public static class AuthMapping
{
    public static LoginResponse ToLoginResponse(this User user, string accessToken, string refreshToken)
    {
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToResponse()
        };
    }

    public static RefreshToken ToEntity(this string token, int userId, DateTime expiresAt)
    {
        return new RefreshToken(token, expiresAt, userId);
    }
}
