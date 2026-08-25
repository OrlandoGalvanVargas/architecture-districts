using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Beacons.DeleteBeacon;

public class DeleteBeaconCommandHandler : IRequestHandler<DeleteBeaconCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteBeaconCommandHandler(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteBeaconCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can delete beacons.");

        var beacon = await _context.Beacons
            .Include(b => b.Faculty)
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (beacon is null)
            throw new NotFoundException(nameof(Beacon), command.Id);

        beacon.Unassign();

        beacon.SoftDelete();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
