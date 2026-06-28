using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.UpdateDistrict
{
    public class UpdateDistrictHandler : IRequestHandler<UpdateDistrictCommand, DistrictResponse?>
    {
        private readonly ApplicationDbContext _context;

        public UpdateDistrictHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DistrictResponse?> Handle(UpdateDistrictCommand command, CancellationToken cancellationToken)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

            if (district is null)
                return null;

            var req = command.Request;

            var codeToken = await _context.Districts.AnyAsync(d => d.Code == req.Code && d.Id != command.Id, cancellationToken);
            if (codeToken)
                throw new InvalidOperationException($"A district with the code '{req.Code}' already exists.");

            district.Name = req.Name;
            district.Code = req.Code;
            district.State = req.State;
            district.City = req.City;
            district.ZipCode = req.ZipCode;
            district.Address = req.Address;
            district.Description = req.Description;
            district.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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
