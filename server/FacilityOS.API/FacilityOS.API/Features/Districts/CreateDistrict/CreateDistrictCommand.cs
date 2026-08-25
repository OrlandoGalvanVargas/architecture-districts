using FacilityOS.API.DTOs.Districts;
using MediatR;

namespace FacilityOS.API.Features.Districts.CreateDistrict;
public record CreateDistrictCommand(CreateDistrictRequest Request) : IRequest<DistrictResponse>;
