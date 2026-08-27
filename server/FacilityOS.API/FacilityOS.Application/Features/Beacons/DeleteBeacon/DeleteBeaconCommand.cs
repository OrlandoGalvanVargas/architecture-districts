using MediatR;

namespace FacilityOS.Application.Features.Beacons.DeleteBeacon;

public record DeleteBeaconCommand(int Id) : IRequest;