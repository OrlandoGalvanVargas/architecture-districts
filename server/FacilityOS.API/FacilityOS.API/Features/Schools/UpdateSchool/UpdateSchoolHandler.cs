using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.UpdateSchool
{
    public class UpdateSchoolHandler : IRequestHandler<UpdateSchoolCommand, SchoolResponse?>
    {
        private readonly ApplicationDbContext _context;

        public UpdateSchoolHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SchoolResponse?> Handle(UpdateSchoolCommand command, CancellationToken cancellationToken)
        {
            var school = await _context.Schools
                .Include(s => s.District)
                .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

            if (school is null)
                return null;

            var req = command.Request;

            var codeToken = await _context.Schools.AnyAsync(s => s.SchoolCode == req.SchoolCode && s.Id != command.Id, cancellationToken);

            if (codeToken)
                throw new InvalidOperationException($"A school with the code '{req.SchoolCode}' already exists");

            var previousDistrictId = school.DistrictId;

            school.Name = req.Name;
            school.SchoolCode = req.SchoolCode;
            school.Level = req.Level;
            school.Type = req.Type;
            school.Address = req.Address;
            school.City = req.City;
            school.State = req.State;
            school.ZipCode = req.ZipCode;
            school.Phone = req.Phone;
            school.ContactEmail = req.ContactEmail;
            school.StudentCapacity = req.StudentCapacity;
            school.IsActive = req.IsActive;
            school.DistrictId = req.DistrictId;
            school.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            if (previousDistrictId != req.DistrictId)
            {
                var previousDistrict = await _context.Districts.FirstOrDefaultAsync(d => d.Id == previousDistrictId, cancellationToken);
                var newDistrict = await _context.Districts.FirstOrDefaultAsync(d => d.Id == req.DistrictId, cancellationToken);

                if (previousDistrict is not null)
                    previousDistrict.SchoolCount = await _context.Schools.CountAsync(s => s.DistrictId == previousDistrictId && s.Id != school.Id, cancellationToken);

                if (newDistrict is not null)
                    newDistrict.SchoolCount = await _context.Schools.CountAsync(s => s.DistrictId == req.DistrictId, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }

            return new SchoolResponse
            {
                Id = command.Id,
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
                StudentCapacity = school.StudentCapacity,
                IsActive = school.IsActive,
                DistrictId = school.DistrictId,
                DistrictName = school.District.Name,
                CreatedAt = school.CreatedAt,
                UpdatedAt = school.UpdatedAt
            };
        }
    }
}
