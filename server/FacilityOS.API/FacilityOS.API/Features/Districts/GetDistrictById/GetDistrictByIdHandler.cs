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
        var canAccess = await _authService.CanAccessDistrictAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have access to view this district.");

        var response = await _context.Districts
                    .AsNoTracking()
                    .Where(d => d.Id == request.Id)
                    .Select(d => new DistrictResponse
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Code = d.Code,
                        State = d.State,
                        City = d.City,
                        ZipCode = d.ZipCode,
                        Address = d.Address,
                        Description = d.Description,
                        SchoolCount = d.Schools.Count(),
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            throw new NotFoundException(nameof(District), request.Id);

        return response;
    }
}