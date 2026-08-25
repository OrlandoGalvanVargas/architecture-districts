using FacilityOS.API.Common;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models.Enums;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.GetSchools;

public class GetSchoolsHandler : IRequestHandler<GetSchoolsQuery, PagedResult<SchoolResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSchoolsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<SchoolResponse>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Schools.AsNoTracking().AsQueryable();

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            query = query.Where(s => s.DistrictId == _currentUser.EntityId.Value);
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            query = query.Where(s => s.Id == _currentUser.EntityId.Value);
        }

        if (request.DistrictId.HasValue)
        {
            query = query.Where(s => s.DistrictId == request.DistrictId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(searchTerm) ||
                                     s.SchoolCode.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(request.Level) && Enum.TryParse<SchoolLevel>(request.Level, true, out var level))
        {
            query = query.Where(s => s.Level == level);
        }

        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<SchoolType>(request.Type, true, out var type))
        {
            query = query.Where(s => s.Type == type);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToResponse() 
            .ToListAsync(cancellationToken);

        return new PagedResult<SchoolResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
