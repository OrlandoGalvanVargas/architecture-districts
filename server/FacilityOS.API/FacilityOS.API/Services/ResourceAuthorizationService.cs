using FacilityOS.API.Common;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Models.Enums;
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

            if (targetRole == AppConstants.Roles.DistrictAdmin) // Usando tus constantes AppRoles de forma consistente
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

    private Task<bool> SchoolBelongsToDistrictAsync(int schoolId, int districtId, CancellationToken cancellationToken)
    {
        return _context.Schools
            .AsNoTracking()
            .AnyAsync(s => s.Id == schoolId && s.DistrictId == districtId, cancellationToken);
    }
}
