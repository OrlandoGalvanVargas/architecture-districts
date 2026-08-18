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

        if (_currentUser.IsDistrictAdmin)
        {
            query = query.Where(d => d.Id == _currentUser.EntityId);
        }
        else if (_currentUser.IsSchoolAdmin)
        {
            query = query.Where(d => d.Schools.Any(s => s.Id == _currentUser.EntityId));
        }

        return await query
            .OrderBy(d => d.Name)
            .Select(d => new DistrictResponse
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                State = d.State,
                City = d.City,
                ZipCode = d.ZipCode,
                Address = d.Address,
                Description = d.Description,
                SchoolCount = d.Schools.Count(),
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}