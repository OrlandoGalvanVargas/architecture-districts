using MediatR;

namespace FacilityOS.API.Features.Districts.DeleteDistrict
{
    public record DeleteDistrictCommand(int Id) : IRequest;
}
