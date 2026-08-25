using FacilityOS.API.Common;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Faculties;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Faculties.GetFaculties;

public class GetFacultiesQueryHandler : IRequestHandler<GetFacultiesQuery, PagedResult<FacultyResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetFacultiesQueryHandler(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<FacultyResponse>> Handle(GetFacultiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Faculties.AsNoTracking().AsQueryable();

        query = ApplyAccessFilter(query);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(f =>
                f.FirstName.ToLower().Contains(searchTerm) ||
                f.LastName.ToLower().Contains(searchTerm) ||
                f.Email.ToLower().Contains(searchTerm));
        }

        if (request.DistrictId.HasValue)
        {
            query = query.Where(f => f.DistrictId == request.DistrictId.Value);
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(f => f.SchoolId == request.SchoolId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(f => f.IsActive == request.IsActive.Value);
        }

        if (request.HasBeacon.HasValue)
        {
            if (request.HasBeacon.Value)
            {
                query = query.Where(f => f.Beacon != null);
            }
            else
            {
                query = query.Where(f => f.Beacon == null);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(f => f.LastName)
            .ThenBy(f => f.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToResponse() 
            .ToListAsync(cancellationToken);

        return new PagedResult<FacultyResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Models.Faculty> ApplyAccessFilter(IQueryable<Models.Faculty> query)
    {
        if (_currentUser.IsAdmin)
            return query;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;
            return query.Where(f =>
                f.DistrictId == districtId ||
                (f.School != null && f.School.DistrictId == districtId));
        }

        if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            var schoolId = _currentUser.EntityId.Value;
            return query.Where(f => f.SchoolId == schoolId);
        }

        return query.Where(f => false);
    }
}
