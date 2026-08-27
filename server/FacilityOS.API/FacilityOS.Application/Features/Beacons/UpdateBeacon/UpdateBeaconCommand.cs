using FacilityOS.Application.DTOs.Beacons;
using MediatR;

namespace FacilityOS.Application.Features.Beacons.UpdateBeacon;

public record UpdateBeaconCommand(int Id, UpdateBeaconRequest Request) : IRequest<BeaconResponse>;