using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.CreateDistrict
{
    public class CreateDistrictHandler : IRequestHandler<CreateDistrictCommand, DistrictResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateDistrictHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DistrictResponse> Handle(CreateDistrictCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            var exists = await _context.Districts
                .AnyAsync(d => d.Code == req.Code, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"A district with code '{req.Code}' already exists.");

            var district = new District
            {
                Name = req.Name,
                Code = req.Code,
                State = req.State,
                City = req.City,
                ZipCode = req.ZipCode,
                Address = req.Address,
                Description = req.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Districts.Add(district);
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
