using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistrictById;

public class GetDistrictByIdHandler : IRequestHandler<GetDistrictByIdQuery, DistrictResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetDistrictByIdHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<DistrictResponse> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
    {
        var districtResponse = await _context.Districts
            .AsNoTracking()
            .Where(d => d.Id == request.Id)
            .ProjectToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        if (districtResponse is null)
            throw new NotFoundException(nameof(District), request.Id);

        var canAccess = await _authService.CanAccessDistrictAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this district.");

        return districtResponse;
    }
}
