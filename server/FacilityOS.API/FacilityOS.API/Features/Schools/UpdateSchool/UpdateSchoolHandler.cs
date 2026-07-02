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

            school.Name = req.Name;
            school.SchoolCode = req.SchoolCode;
            school.Address = req.Address;
            school.City = req.City;
            school.State = req.State;
            school.ZipCode = req.ZipCode;
            school.Phone = req.Phone;
            school.ContactEmail = req.ContactEmail;
            school.StudenCapacity = req.StudentCapacity;
            school.isActive = req.IsActive;
            school.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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
                StudentCapacity = school.StudenCapacity,
                IsActive = school.isActive,
                DistrictId = school.DistrictId,
                DistrictName = school.District.Name,
                CreatedAt = school.CreatedAt,
                UpdatedAt = school.UpdatedAt
            };
        }
    }
}
