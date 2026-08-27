using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Beacons;
using FacilityOS.Domain.Models.Enums;
using MediatR;

namespace FacilityOS.Application.Features.Beacons.GetBeacons;

public record GetBeaconsQuery(
    string? Search = null,
    BeaconType? Type = null,
    BeaconStatus? Status = null,
    int? DistrictId = null,
    int? SchoolId = null,
    bool? IsAssigned = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<BeaconResponse>>;