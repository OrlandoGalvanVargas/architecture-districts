using FacilityOS.Application.DTOs.Beacons;
using MediatR;

namespace FacilityOS.Application.Features.Beacons.CreateBeacon;

public record CreateBeaconCommand(CreateBeaconRequest Request) : IRequest<BeaconResponse>;