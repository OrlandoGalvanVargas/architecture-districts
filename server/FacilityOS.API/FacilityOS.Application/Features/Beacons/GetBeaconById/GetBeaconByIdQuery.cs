using FacilityOS.Application.DTOs.Beacons;
using MediatR;

namespace FacilityOS.Application.Features.Beacons.GetBeaconById;

public record GetBeaconByIdQuery(int Id) : IRequest<BeaconResponse>;