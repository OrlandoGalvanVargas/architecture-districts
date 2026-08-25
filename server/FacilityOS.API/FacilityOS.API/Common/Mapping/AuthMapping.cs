using FacilityOS.API.DTOs.Auth;
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
}
