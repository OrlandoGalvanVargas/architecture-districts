using FacilityOS.API.Common.Exceptions;
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
        var district = await _context.Districts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), request.Id);

        var canAccess = await _authService.CanAccessDistrictAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have access to view this district.");

        return new DistrictResponse
        {
            Id = district.Id,
            Name = district.Name,
            Code = district.Code,
            State = district.State,
            City = district.City,
            ZipCode = district.ZipCode,
            Address = district.Address,
            Description = district.Description,
            SchoolCount = district.Schools.Count(),
            CreatedAt = district.CreatedAt,
            UpdatedAt = district.UpdatedAt
        };
    }
}