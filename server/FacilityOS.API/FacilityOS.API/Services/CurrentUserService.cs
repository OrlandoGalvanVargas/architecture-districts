using System.Security.Claims;
using FacilityOS.API.Common;

namespace FacilityOS.API.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Role { get; }
    int? EntityId { get; }
    string? EntityType { get; }

    bool IsAdmin => Role == AppRoles.Admin;
    bool IsDistrictAdmin => Role == AppRoles.DistrictAdmin;
    bool IsSchoolAdmin => Role == AppRoles.SchoolAdmin;
}

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
    public string? EntityType => User?.FindFirstValue(AppClaimTypes.EntityType);
    public int? EntityId => int.TryParse(User?.FindFirstValue(AppClaimTypes.EntityId), out var id) ? id : null;
}