using FacilityOS.Application.DTOs.Districts;
using MediatR;

namespace FacilityOS.Application.Features.Districts.GetDistricts
{
    public record GetDistrictsQuery() : IRequest<List<DistrictResponse>>;
}
