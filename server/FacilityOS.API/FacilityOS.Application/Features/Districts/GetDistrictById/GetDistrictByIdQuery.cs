using FacilityOS.Application.DTOs.Districts;
using MediatR;

namespace FacilityOS.Application.Features.Districts.GetDistrictById;

public record GetDistrictByIdQuery(int Id) : IRequest<DistrictResponse>;
