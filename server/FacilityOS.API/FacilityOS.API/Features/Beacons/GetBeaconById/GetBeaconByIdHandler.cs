using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Beacons;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Beacons.GetBeaconById;

public class GetBeaconByIdHandler : IRequestHandler<GetBeaconByIdQuery, BeaconResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetBeaconByIdHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<BeaconResponse> Handle(GetBeaconByIdQuery request, CancellationToken cancellationToken)
    {.
        var beaconResponse = await _context.Beacons
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
            .ProjectToResponse() 
            .FirstOrDefaultAsync(cancellationToken);

        if (beaconResponse is null)
            throw new NotFoundException(nameof(Beacon), request.Id);

        var canAccess = await _authService.CanAccessBeaconAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this beacon.");

        return beaconResponse;
    }
}
