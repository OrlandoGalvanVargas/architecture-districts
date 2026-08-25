using FacilityOS.API.DTOs.Beacons;
using MediatR;

namespace FacilityOS.API.Features.Beacons.UpdateBeacon;

public record UpdateBeaconCommand(int Id, UpdateBeaconRequest Request) : IRequest<BeaconResponse>;