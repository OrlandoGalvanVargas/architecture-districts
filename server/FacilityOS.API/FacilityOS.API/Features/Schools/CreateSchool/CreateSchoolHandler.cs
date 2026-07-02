using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.CreateSchool
{
    public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand,SchoolResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateSchoolHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SchoolResponse> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            var districtExists = await _context.Districts.AnyAsync(d => d.Id == req.DistrictId, cancellationToken);

            if (!districtExists)
                throw new InvalidOperationException($"District with Id '{req.DistrictId}' does not exist");

            var codeExists = await _context.Schools.AnyAsync(s => s.SchoolCode == req.SchoolCode, cancellationToken);

            if (codeExists)
                throw new InvalidOperationException($"A school with code '{req.SchoolCode}' already exists");

            var school = new School
            {
                Name = req.Name,
                SchoolCode = req.SchoolCode,
                Level = req.Level,
                Type = req.Type,
                Address = req.Address,
                City = req.City,
                State = req.State,
                ZipCode = req.ZipCode,
                Phone = req.Phone,
                ContactEmail = req.ContactEmail,
                StudenCapacity = req.StudentCapacity,
                DistrictId = req.DistrictId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Schools.Add(school);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(school).Reference(s => s.District).LoadAsync(cancellationToken);

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
                UpdatedAt = school.UpdatedAt
            };
        }
    }
}
