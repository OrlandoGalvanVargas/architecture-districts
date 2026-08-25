using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistricts;

public class GetDistrictsHandler : IRequestHandler<GetDistrictsQuery, List<DistrictResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDistrictsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
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
