using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Faculties;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using FacilityOS.Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Faculties.CreateFaculty;

public class CreateFacultyHandler : IRequestHandler<CreateFacultyCommand, FacultyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IResourceAuthorizationService _authService;

    public CreateFacultyHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IResourceAuthorizationService authService)
    {
        _context = context;
        _currentUser = currentUser;
        _authService = authService;
    }

    public async Task<FacultyResponse> Handle(CreateFacultyCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        await ValidateCreatePermissionAsync(req, cancellationToken);

        var emailExists = await _context.Faculties
            .AnyAsync(f => f.Email.ToLower() == req.Email.ToLower().Trim(), cancellationToken);

        if (emailExists)
            throw new ConflictException($"A faculty with email '{req.Email}' already exists.");

        if (req.DistrictId.HasValue)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == req.DistrictId.Value, cancellationToken);
            if (district is null) throw new NotFoundException(nameof(District), req.DistrictId.Value);
        }
        if (req.SchoolId.HasValue)
        {
            var school = await _context.Schools.FirstOrDefaultAsync(s => s.Id == req.SchoolId.Value, cancellationToken);
            if (school is null) throw new NotFoundException(nameof(School), req.SchoolId.Value);
        }

        Beacon? beacon = null;
        if (req.BeaconId.HasValue)
        {
            var canAssignBeacon = await _authService.CanAssignBeaconToFacultyAsync(req.BeaconId.Value, cancellationToken);
            if (!canAssignBeacon)
                throw new ForbiddenException("You do not have permission to handle or assign this beacon.");

            beacon = await _context.Beacons.FirstOrDefaultAsync(b => b.Id == req.BeaconId.Value, cancellationToken);
            if (beacon is null)
                throw new NotFoundException(nameof(Beacon), req.BeaconId.Value);

            if (beacon.Status != BeaconStatus.Available || beacon.FacultyId.HasValue)
                throw new ConflictException($"Beacon '{beacon.DeviceName}' with Serial ({beacon.SerialNumber}) is already assigned, inactive or in maintenance.");
        }

        var faculty = req.ToEntity();
        _context.Faculties.Add(faculty);

        await _context.SaveChangesAsync(cancellationToken);

        if (beacon is not null)
        {
            beacon.AssignToFaculty(faculty.Id);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return faculty.ToResponse();
    }

    private async Task ValidateCreatePermissionAsync(CreateFacultyRequest req, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin) return;

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            if (req.DistrictId.HasValue)
            {
                if (req.DistrictId.Value != _currentUser.EntityId.Value)
                    throw new ForbiddenException("You can only create faculties in your own district.");
            }
            else if (req.SchoolId.HasValue)
            {
                var schoolBelongs = await _context.Schools
                    .AnyAsync(s => s.Id == req.SchoolId.Value && s.DistrictId == _currentUser.EntityId.Value, cancellationToken);

                if (!schoolBelongs)
                    throw new ForbiddenException("You can only create faculties in schools within your district.");
            }
            else
            {
                throw new ForbiddenException("Faculty must be assigned to a district or school.");
            }
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            if (!req.SchoolId.HasValue || req.SchoolId.Value != _currentUser.EntityId.Value)
                throw new ForbiddenException("You can only create faculties in your own school.");

            if (req.DistrictId.HasValue)
                throw new ForbiddenException("SchoolAdmin cannot assign faculty directly to a district.");
        }
        else
        {
            throw new ForbiddenException("You do not have permission to create faculties.");
        }
    }
}
