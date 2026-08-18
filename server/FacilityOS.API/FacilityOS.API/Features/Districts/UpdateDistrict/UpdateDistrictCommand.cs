using FacilityOS.API.DTOs.Districts;
using MediatR;

namespace FacilityOS.API.Features.Districts.UpdateDistrict
{
    public record UpdateDistrictCommand(int Id, UpdateDistrictRequest Request) : IRequest<DistrictResponse>;
}
