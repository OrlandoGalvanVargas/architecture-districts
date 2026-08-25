using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Faculties;
using FacilityOS.API.Models;
using FacilityOS.API.Models.Enums;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Faculties.UpdateFaculty;

public class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyCommand, FacultyResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public UpdateFacultyCommandHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<FacultyResponse> Handle(UpdateFacultyCommand command, CancellationToken cancellationToken)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (faculty is null)
            throw new NotFoundException(nameof(Faculty), command.Id);

        var canManage = await _authService.CanManageFacultyAsync(command.Id, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to modify this faculty.");

        var req = command.Request;

        if (!faculty.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _context.Faculties
                .AnyAsync(f => f.Email.ToLower() == req.Email.ToLower().Trim() && f.Id != command.Id, cancellationToken);

            if (emailExists)
                throw new ConflictException($"A faculty with email '{req.Email}' already exists.");
        }

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

        var currentBeacon = await _context.Beacons
            .FirstOrDefaultAsync(b => b.FacultyId == faculty.Id, cancellationToken);

        if (req.BeaconId != currentBeacon?.Id)
        {
            if (currentBeacon is not null)
            {
                currentBeacon.Unassign(); 
            }

            if (req.BeaconId.HasValue)
            {
                var canAssignBeacon = await _authService.CanAssignBeaconToFacultyAsync(req.BeaconId.Value, cancellationToken);
                if (!canAssignBeacon)
                    throw new ForbiddenException("You do not have permission to handle or assign this beacon.");

                var newBeacon = await _context.Beacons
                    .FirstOrDefaultAsync(b => b.Id == req.BeaconId.Value, cancellationToken);

                if (newBeacon is null)
                    throw new NotFoundException(nameof(Beacon), req.BeaconId.Value);

                if (newBeacon.Status != BeaconStatus.Available || newBeacon.FacultyId.HasValue)
                    throw new ConflictException($"Beacon '{newBeacon.DeviceName}' is already assigned, inactive or in maintenance.");

                newBeacon.AssignToFaculty(faculty.Id);
            }
        }

        faculty.UpdateFromRequest(req);

        if (req.IsActive != faculty.IsActive)
        {
            if (req.IsActive)
                faculty.Activate();   
            else
                faculty.Deactivate(); 
        }
        await _context.SaveChangesAsync(cancellationToken);

        var updatedFaculty = await _context.Faculties
            .Include(f => f.District)
            .Include(f => f.School)
            .Include(f => f.Beacon)
            .FirstAsync(f => f.Id == faculty.Id, cancellationToken);

        return updatedFaculty.ToResponse();
    }
}
