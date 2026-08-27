using FacilityOS.Application.Common;
using FacilityOS.Application.Services;
using System.Security.Claims;

namespace FacilityOS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId => int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string? Role => User?.FindFirstValue(ClaimTypes.Role);
    public string? EntityType => User?.FindFirstValue(AppConstants.Claims.EntityType);
    public int? EntityId => int.TryParse(User?.FindFirstValue(AppConstants.Claims.EntityId), out var id) ? id : null;
}