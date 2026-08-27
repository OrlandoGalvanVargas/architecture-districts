using MediatR;

namespace FacilityOS.Application.Features.Districts.DeleteDistrict;

public record DeleteDistrictCommand(int Id) : IRequest;