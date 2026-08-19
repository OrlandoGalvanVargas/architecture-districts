using FacilityOS.API.Common;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
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

        if (_currentUser.IsDistrictAdmin)
        {
            return await _context.Schools
                .AsNoTracking()
                .AnyAsync(s => s.Id == schoolId && s.DistrictId == _currentUser.EntityId, cancellationToken);
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

        if (_currentUser.IsDistrictAdmin)
        {
            return await _context.Schools
                .AsNoTracking()
                .AnyAsync(s => s.Id == schoolId && s.DistrictId == _currentUser.EntityId, cancellationToken);
        }

        return false;
    }

    public bool CanCreateSchoolInDistrict(int targetDistrictId)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin)
        {
            return _currentUser.EntityId == targetDistrictId;
        }

        return false;
    }

    public async Task<bool> CanAccessDistrictAsync(int districtId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin) return true;

        if (_currentUser.IsDistrictAdmin)
        {
            return _currentUser.EntityId == districtId;
        }

        if (_currentUser.IsSchoolAdmin)
        {
            return await _context.Schools
                .AsNoTracking()
                .AnyAsync(s => s.Id == _currentUser.EntityId && s.DistrictId == districtId, cancellationToken);
        }

        return false;
    }

    public async Task<bool> CanCreateUserRoleAsync( string targetRole, UserEntityType targetEntityType, int? targetEntityId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsAdmin)
            return true;

        if (_currentUser.IsDistrictAdmin)
        {
            if (targetRole == AppRoles.Admin)
                return false;

            if (targetEntityType == UserEntityType.District)
            {
                return targetEntityId == _currentUser.EntityId;
            }

            if (targetEntityType == UserEntityType.School)
            {
                if (!targetEntityId.HasValue)
                    return false;

                return await _context.Schools
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == targetEntityId.Value && s.DistrictId == _currentUser.EntityId, cancellationToken);
            }

            return false;
        }

        if (_currentUser.IsSchoolAdmin)
        {
            if (targetRole == AppRoles.Admin || targetRole == AppRoles.DistrictAdmin || targetRole == AppRoles.SchoolAdmin)
                return false;

            if (targetEntityType == UserEntityType.School)
            {
                return targetEntityId == _currentUser.EntityId;
            }

            return false;
        }

        return false;
    }

    public async Task<bool> ValidateEntityExistsAsync(UserEntityType entityType, int? entityId, CancellationToken cancellationToken = default)
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

        if (targetUser.Role == AppRoles.Admin) return false;

        if (_currentUser.IsDistrictAdmin)
        {
            if (targetUser.EntityType == UserEntityType.District && targetUser.EntityId == _currentUser.EntityId)
                return true;

            if (targetUser.EntityType == UserEntityType.School && targetUser.EntityId.HasValue)
            {
                return await _context.Schools
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == targetUser.EntityId.Value && s.DistrictId == _currentUser.EntityId, cancellationToken);
            }
        }

        if (_currentUser.IsSchoolAdmin)
        {
            if (targetUser.Role == AppRoles.SchoolAdmin || targetUser.Role == AppRoles.DistrictAdmin)
                return false;

            if (targetUser.EntityType == UserEntityType.School && targetUser.EntityId == _currentUser.EntityId)
                return true;
        }

        return false;
    }
}