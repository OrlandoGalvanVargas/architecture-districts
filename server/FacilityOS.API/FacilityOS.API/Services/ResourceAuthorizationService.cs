using FacilityOS.API.Data;
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
}