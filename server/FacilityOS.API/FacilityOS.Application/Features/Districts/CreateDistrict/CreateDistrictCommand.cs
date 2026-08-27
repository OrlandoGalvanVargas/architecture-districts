using FacilityOS.Application.DTOs.Districts;
using MediatR;

namespace FacilityOS.Application.Features.Districts.CreateDistrict;

public record CreateDistrictCommand(CreateDistrictRequest Request) : IRequest<DistrictResponse>;
