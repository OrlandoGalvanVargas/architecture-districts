using FacilityOS.API.Models;
using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.Services;

public interface IResourceAuthorizationService
{
    Task<bool> CanAccessSchoolAsync(int schoolId, CancellationToken cancellationToken = default);
    Task<bool> CanManageSchoolAsync(int schoolId, CancellationToken cancellationToken = default);
    Task<bool> CanCreateSchoolInDistrictAsync(int targetDistrictId, CancellationToken cancellationToken = default);

    Task<bool> CanAccessDistrictAsync(int districtId, CancellationToken cancellationToken = default);

    Task<bool> CanCreateUserRoleAsync(string targetRole, UserEntityType targetEntityType, int? targetEntityId, CancellationToken cancellationToken = default);
    Task<bool> ValidateEntityExistsAsync(UserEntityType entityType, int? entityId, CancellationToken cancellationToken = default);
    Task<bool> CanManageUserAsync(User targetUser, CancellationToken cancellationToken = default);

    Task<bool> CanAccessBeaconAsync(int beaconId, CancellationToken cancellationToken = default);

    Task<bool> CanAccessFacultyAsync(int facultyId, CancellationToken cancellationToken = default);
    Task<bool> CanManageFacultyAsync(int facultyId, CancellationToken cancellationToken = default);
    Task<bool> CanAssignBeaconToFacultyAsync(int beaconId, CancellationToken cancellationToken = default);
}