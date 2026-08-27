using FacilityOS.Domain.Models;

namespace FacilityOS.Application.Services;

public interface IAuthService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}