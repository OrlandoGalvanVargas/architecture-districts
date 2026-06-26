using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistricts
{
    public class GetDistrictsHandler : IRequestHandler<GetDistrictsQuery, List<DistrictResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetDistrictsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DistrictResponse>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Districts
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
                    SchoolCount = d.SchoolCount,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
