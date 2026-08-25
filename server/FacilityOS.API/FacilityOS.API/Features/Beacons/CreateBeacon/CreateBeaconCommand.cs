using FacilityOS.API.DTOs.Beacons;
using MediatR;

namespace FacilityOS.API.Features.Beacons.CreateBeacon;

public record CreateBeaconCommand(CreateBeaconRequest Request) : IRequest<BeaconResponse>;