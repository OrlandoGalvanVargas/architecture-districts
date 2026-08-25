using FacilityOS.API.Common;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Beacons;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Beacons.GetBeacons;

public class GetBeaconsHandler : IRequestHandler<GetBeaconsQuery, PagedResult<BeaconResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBeaconsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<BeaconResponse>> Handle(GetBeaconsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Beacons.AsNoTracking().AsQueryable();

        query = ApplyAccessFilter(query);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(b =>
                b.DeviceName.ToLower().Contains(searchTerm) ||
                b.SerialNumber.ToLower().Contains(searchTerm));
        }

        if (request.Type.HasValue)
        {
            query = query.Where(b => b.Type == request.Type.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        if (request.DistrictId.HasValue)
        {
            query = query.Where(b => b.DistrictId == request.DistrictId.Value);
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(b => b.SchoolId == request.SchoolId.Value);
        }

        if (request.IsAssigned.HasValue)
        {
            if (request.IsAssigned.Value)
            {
                query = query.Where(b => b.DistrictId.HasValue || b.SchoolId.HasValue || b.FacultyId.HasValue);
            }
            else
            {
                query = query.Where(b => !b.DistrictId.HasValue && !b.SchoolId.HasValue && !b.FacultyId.HasValue);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToResponse() 
            .ToListAsync(cancellationToken);

        return new PagedResult<BeaconResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Models.Beacon> ApplyAccessFilter(IQueryable<Models.Beacon> query)
    {
        if (_currentUser.IsAdmin)
            return query;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;
            return query.Where(b =>
                b.DistrictId == districtId ||
                (b.School != null && b.School.DistrictId == districtId) ||
                (b.Faculty != null && b.Faculty.DistrictId == districtId) ||
                (b.Faculty != null && b.Faculty.School != null && b.Faculty.School.DistrictId == districtId));
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            var schoolId = _currentUser.EntityId.Value;
            return query.Where(b =>
                b.SchoolId == schoolId ||
                (b.Faculty != null && b.Faculty.SchoolId == schoolId));
        }

        return query.Where(b => false);
    }
}
