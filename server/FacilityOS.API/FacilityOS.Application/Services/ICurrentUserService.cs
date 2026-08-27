using FacilityOS.Application.Common;

namespace FacilityOS.Application.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Role { get; }
        int? EntityId { get; }
        string? EntityType { get; }

        bool IsAdmin => Role == AppConstants.Roles.Admin;
        bool IsDistrictAdmin => Role == AppConstants.Roles.DistrictAdmin;
        bool IsSchoolAdmin => Role == AppConstants.Roles.SchoolAdmin;
    }
}
