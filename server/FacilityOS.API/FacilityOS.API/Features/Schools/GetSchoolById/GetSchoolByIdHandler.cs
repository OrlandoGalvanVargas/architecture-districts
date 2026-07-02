using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.GetSchoolById
{
    public class GetSchoolByIdHandler : IRequestHandler<GetSchoolByIdQuery, SchoolResponse?>
    {
        private readonly ApplicationDbContext _context;

        public GetSchoolByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SchoolResponse?> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
        {
            var school = await _context.Schools
                .Include(s => s.District)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (school is null)
                return null;

            return new SchoolResponse
            {
                Id = school.Id,
                Name = school.Name,
                SchoolCode = school.SchoolCode,
                Level = school.Level.ToString(),
                Type = school.Type.ToString(),
                Address = school.Address,
                City = school.City,
                State = school.State,
                ZipCode = school.ZipCode,
                Phone = school.Phone,
                ContactEmail = school.ContactEmail,
                StudentCapacity = school.StudenCapacity,
                IsActive = school.isActive,
                DistrictId = school.DistrictId,
                DistrictName = school.District.Name,
                CreatedAt = school.CreatedAt,
                UpdatedAt = school.UpdatedAt,
            };
        }
    }
}
