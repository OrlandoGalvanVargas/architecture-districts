using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistrictById
{
    public class GetDistrictByIdHandler : IRequestHandler<GetDistrictByIdQuery, DistrictResponse?>
    {
        private readonly ApplicationDbContext _context;

        public GetDistrictByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DistrictResponse?> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            
            if (district is null) 
                return null;

            return new DistrictResponse
            {
                Id = district.Id,
                Name = district.Name,
                Code = district.Code,
                State = district.State,
                City = district.City,
                ZipCode = district.ZipCode,
                Address = district.Address,
                Description = district.Description,
                SchoolCount = district.SchoolCount,
                CreatedAt = district.CreatedAt,
                UpdatedAt = district.UpdatedAt
            };

        }
    }
}
