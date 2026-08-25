using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Beacons;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Beacons.CreateBeacon;

public class CreateBeaconHandler : IRequestHandler<CreateBeaconCommand, BeaconResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBeaconHandler(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BeaconResponse> Handle(CreateBeaconCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can create beacons.");

        var req = command.Request;

        var serialNumberClean = req.SerialNumber.Trim().ToUpper();
        var serialExists = await _context.Beacons
            .AnyAsync(b => b.SerialNumber == serialNumberClean, cancellationToken);

        if (serialExists)
            throw new ConflictException($"A beacon with serial number '{req.SerialNumber}' already exists.");

        if (req.DistrictId.HasValue)
        {
            var district = await _context.Districts
                .FirstOrDefaultAsync(d => d.Id == req.DistrictId.Value, cancellationToken);

            if (district is null)
                throw new NotFoundException(nameof(District), req.DistrictId.Value);
        }

        if (req.SchoolId.HasValue)
        {
            var school = await _context.Schools
                .FirstOrDefaultAsync(s => s.Id == req.SchoolId.Value, cancellationToken);

            if (school is null)
                throw new NotFoundException(nameof(School), req.SchoolId.Value);
        }

        var beacon = req.ToEntity();

        _context.Beacons.Add(beacon);
        await _context.SaveChangesAsync(cancellationToken);

        return beacon.ToResponse();
    }
}
