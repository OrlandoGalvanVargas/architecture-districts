using FacilityOS.API.DTOs.Beacons;
using MediatR;

namespace FacilityOS.API.Features.Beacons.GetBeaconById;

public record GetBeaconByIdQuery(int Id) : IRequest<BeaconResponse>;