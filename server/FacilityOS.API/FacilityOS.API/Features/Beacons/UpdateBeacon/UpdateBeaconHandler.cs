using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Beacons;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Beacons.UpdateBeacon;

public class UpdateBeaconHandler : IRequestHandler<UpdateBeaconCommand, BeaconResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateBeaconHandler(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BeaconResponse> Handle(UpdateBeaconCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can update beacons.");

        var beacon = await _context.Beacons
            .Include(b => b.Faculty) 
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (beacon is null)
            throw new NotFoundException(nameof(Beacon), command.Id);

        var req = command.Request;

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


        beacon.UpdateFromRequest(req);

        await _context.SaveChangesAsync(cancellationToken);

        return beacon.ToResponse();
    }
}
