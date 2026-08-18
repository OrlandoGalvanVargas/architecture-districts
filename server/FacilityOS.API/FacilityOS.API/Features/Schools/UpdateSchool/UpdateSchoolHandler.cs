using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.UpdateSchool;

public class UpdateSchoolHandler : IRequestHandler<UpdateSchoolCommand, SchoolResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateSchoolHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SchoolResponse> Handle(UpdateSchoolCommand command, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .Include(s => s.District)
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), command.Id);

        var req = command.Request;

        var codeTaken = await _context.Schools.AnyAsync(s => s.SchoolCode == req.SchoolCode && s.Id != command.Id, cancellationToken);
        if (codeTaken)
            throw new InvalidOperationException($"A school with the code '{req.SchoolCode}' already exists.");

        if (school.DistrictId != req.DistrictId)
        {
            var targetDistrictExists = await _context.Districts.AnyAsync(d => d.Id == req.DistrictId, cancellationToken);
            if (!targetDistrictExists)
                throw new NotFoundException(nameof(District), req.DistrictId);
        }

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
            StudentCapacity = school.StudentCapacity,
            IsActive = school.IsActive,
            DistrictId = school.DistrictId,
            DistrictName = school.District.Name,
            CreatedAt = school.CreatedAt,
            UpdatedAt = school.UpdatedAt
        };
    }
}