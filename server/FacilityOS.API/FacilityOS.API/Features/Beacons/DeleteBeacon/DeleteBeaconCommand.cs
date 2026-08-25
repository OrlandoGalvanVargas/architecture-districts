using MediatR;

namespace FacilityOS.API.Features.Beacons.DeleteBeacon;

public record DeleteBeaconCommand(int Id) : IRequest;