using FacilityOS.API.DTOs.Districts;
using MediatR;

namespace FacilityOS.API.Features.Districts.GetDistrictById;
public record GetDistrictByIdQuery(int Id) : IRequest<DistrictResponse>;
