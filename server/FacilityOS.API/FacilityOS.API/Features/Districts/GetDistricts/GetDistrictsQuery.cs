using FacilityOS.API.DTOs.Districts;
using MediatR;

namespace FacilityOS.API.Features.Districts.GetDistricts
{
    public record GetDistrictsQuery() : IRequest<List<DistrictResponse>>;
}
