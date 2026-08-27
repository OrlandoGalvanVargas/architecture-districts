using FacilityOS.Application.DTOs.Districts;
using MediatR;

namespace FacilityOS.Application.Features.Districts.UpdateDistrict;

public record UpdateDistrictCommand(int Id, UpdateDistrictRequest Request) : IRequest<DistrictResponse>;
