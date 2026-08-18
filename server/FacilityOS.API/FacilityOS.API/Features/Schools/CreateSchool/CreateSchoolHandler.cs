using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.CreateSchool;

public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand, SchoolResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public CreateSchoolHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (!_authService.CanCreateSchoolInDistrict(req.DistrictId))
            throw new ForbiddenException("You do not have permission to create a school in this district.");

        var districtExists = await _context.Districts.AnyAsync(d => d.Id == req.DistrictId, cancellationToken);
        if (!districtExists)
            throw new NotFoundException(nameof(District), req.DistrictId);

        var codeExists = await _context.Schools.AnyAsync(s => s.SchoolCode == req.SchoolCode, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException($"A school with code '{req.SchoolCode}' already exists.");

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
            StudentCapacity = req.StudentCapacity,
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
            StudentCapacity = school.StudentCapacity,
            IsActive = school.IsActive,
            DistrictId = school.DistrictId,
            DistrictName = school.District.Name,
            CreatedAt = school.CreatedAt,
            UpdatedAt = school.UpdatedAt
        };
    }
}