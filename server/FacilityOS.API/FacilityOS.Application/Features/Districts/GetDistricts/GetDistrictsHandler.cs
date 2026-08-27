using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Districts;
using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Districts.GetDistricts;

public class GetDistrictsHandler : IRequestHandler<GetDistrictsQuery, List<DistrictResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDistrictsHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<DistrictResponse>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Districts.AsNoTracking().AsQueryable();

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;
            query = query.Where(d => d.Id == districtId);
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            var schoolId = _currentUser.EntityId.Value;
            query = query.Where(d => d.Schools.Any(s => s.Id == schoolId));
        }

        return await query
            .OrderBy(d => d.Name)
            .ProjectToResponse()
            .ToListAsync(cancellationToken);
    }
}
