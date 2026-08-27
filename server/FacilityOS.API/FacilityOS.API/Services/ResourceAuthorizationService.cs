using FacilityOS.API.Data;
using FacilityOS.Application.Common;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using FacilityOS.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Services;

public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ResourceAuthorizationService(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> CanAccessSchoolAsync(int schoolId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            return await SchoolBelongsToDistrictAsync(schoolId, _currentUser.EntityId.Value, cancellationToken);
        }

        if (_currentUser.IsSchoolAdmin)
        {
            return _currentUser.EntityId == schoolId;
        }

        return false;
    }

    public async Task<bool> CanManageSchoolAsync(int schoolId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            return await SchoolBelongsToDistrictAsync(schoolId, _currentUser.EntityId.Value, cancellationToken);
        }

        if (_currentUser.IsSchoolAdmin)
        {
            return _currentUser.EntityId == schoolId;
        }

        return false;
    }

    public async Task<bool> CanCreateSchoolInDistrictAsync(int targetDistrictId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin)
        {
            return _currentUser.EntityId == targetDistrictId;
        }

        return await Task.FromResult(false);
    }

    public async Task<bool> CanAccessDistrictAsync(int districtId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin)
        {
            return _currentUser.EntityId == districtId;
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            return await _context.Schools
                .AsNoTracking()
                .AnyAsync(s => s.Id == _currentUser.EntityId.Value && s.DistrictId == districtId, cancellationToken);
        }

        return false;
    }

    public async Task<bool> CanCreateUserRoleAsync(
        string targetRole,
        UserEntityType targetEntityType,
        int? targetEntityId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            if (targetRole == AppConstants.Roles.Admin)
                return false;

            if (targetRole == AppConstants.Roles.SchoolAdmin || targetRole == "User")
            {
                if (targetEntityType == UserEntityType.School && targetEntityId.HasValue)
                {
                    return await SchoolBelongsToDistrictAsync(targetEntityId.Value, _currentUser.EntityId.Value, cancellationToken);
                }
            }

            if (targetRole == AppConstants.Roles.DistrictAdmin)
            {
                return targetEntityType == UserEntityType.District && targetEntityId == _currentUser.EntityId;
            }

            return false;
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            if (targetRole == AppConstants.Roles.SchoolAdmin || targetRole == "User")
            {
                if (targetEntityType == UserEntityType.School)
                {
                    return targetEntityId == _currentUser.EntityId;
                }
            }

            return false;
        }

        return false;
    }

    public async Task<bool> ValidateEntityExistsAsync(
        UserEntityType entityType,
        int? entityId,
        CancellationToken cancellationToken = default)
    {
        if (entityType == UserEntityType.Global)
            return true;

        if (!entityId.HasValue)
            return false;

        if (entityType == UserEntityType.District)
        {
            return await _context.Districts
                .AsNoTracking()
                .AnyAsync(d => d.Id == entityId.Value, cancellationToken);
        }

        if (entityType == UserEntityType.School)
        {
            return await _context.Schools
                .AsNoTracking()
                .AnyAsync(s => s.Id == entityId.Value, cancellationToken);
        }

        return false;
    }

    public async Task<bool> CanManageUserAsync(User targetUser, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;
        if (targetUser.Role == AppConstants.Roles.Admin) return false;
        if (targetUser.EntityType == UserEntityType.Global) return false;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            if (targetUser.EntityType == UserEntityType.District && targetUser.EntityId == _currentUser.EntityId)
                return true;

            if (targetUser.EntityType == UserEntityType.School && targetUser.EntityId.HasValue)
            {
                return await SchoolBelongsToDistrictAsync(targetUser.EntityId.Value, _currentUser.EntityId.Value, cancellationToken);
            }
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            if (targetUser.Role == AppConstants.Roles.SchoolAdmin || targetUser.Role == "User")
            {
                if (targetUser.EntityType == UserEntityType.School && targetUser.EntityId == _currentUser.EntityId)
                    return true;
            }
        }

        return false;
    }

    public async Task<bool> CanAccessBeaconAsync(int beaconId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        var beaconInfo = await _context.Beacons
            .AsNoTracking()
            .Where(b => b.Id == beaconId)
            .Select(b => new
            {
                b.DistrictId,
                b.SchoolId,
                b.FacultyId,
                FacultyDistrictId = b.Faculty != null ? b.Faculty.DistrictId : null,
                FacultySchoolId = b.Faculty != null ? b.Faculty.SchoolId : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (beaconInfo is null) return false;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var currentDistrictId = _currentUser.EntityId.Value;

            if (beaconInfo.DistrictId == currentDistrictId || beaconInfo.FacultyDistrictId == currentDistrictId)
                return true;

            if (beaconInfo.SchoolId.HasValue)
                return await SchoolBelongsToDistrictAsync(beaconInfo.SchoolId.Value, currentDistrictId, cancellationToken);

            if (beaconInfo.FacultySchoolId.HasValue)
                return await SchoolBelongsToDistrictAsync(beaconInfo.FacultySchoolId.Value, currentDistrictId, cancellationToken);
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            var currentSchoolId = _currentUser.EntityId.Value;

            return beaconInfo.SchoolId == currentSchoolId || beaconInfo.FacultySchoolId == currentSchoolId;
        }

        return false;
    }

    public async Task<bool> CanAccessFacultyAsync(int facultyId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        var faculty = await _context.Faculties
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == facultyId, cancellationToken);

        if (faculty is null) return false;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            if (faculty.DistrictId == _currentUser.EntityId) return true;

            if (faculty.SchoolId.HasValue)
                return await SchoolBelongsToDistrictAsync(faculty.SchoolId.Value, _currentUser.EntityId.Value, cancellationToken);
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            return faculty.SchoolId == _currentUser.EntityId;
        }

        return false;
    }

    public async Task<bool> CanManageFacultyAsync(int facultyId, CancellationToken cancellationToken = default)
    {
        return await CanAccessFacultyAsync(facultyId, cancellationToken);
    }

    public async Task<bool> CanAssignBeaconToFacultyAsync(int beaconId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        var beacon = await _context.Beacons
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == beaconId, cancellationToken);

        if (beacon is null) return false;

        if (beacon.Status == BeaconStatus.Inactive || beacon.Status == BeaconStatus.Maintenance)
            return false;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            if (beacon.DistrictId == _currentUser.EntityId) return true;

            if (beacon.SchoolId.HasValue)
                return await SchoolBelongsToDistrictAsync(beacon.SchoolId.Value, _currentUser.EntityId.Value, cancellationToken);
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            return beacon.SchoolId == _currentUser.EntityId;
        }

        return false;
    }

    private Task<bool> SchoolBelongsToDistrictAsync(int schoolId, int districtId, CancellationToken cancellationToken)
    {
        return _context.Schools
            .AsNoTracking()
            .AnyAsync(s => s.Id == schoolId && s.DistrictId == districtId, cancellationToken);
    }
}