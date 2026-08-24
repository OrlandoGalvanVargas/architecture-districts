using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.CreateDistrict;

public class CreateDistrictHandler : IRequestHandler<CreateDistrictCommand, DistrictResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateDistrictHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DistrictResponse> Handle(CreateDistrictCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can create new districts.");

        // Accedemos a las propiedades a través de command.Request
        var req = command.Request;

        var exists = await _context.Districts
            .AnyAsync(d => d.Code.ToLower() == req.Code.ToLower().Trim(), cancellationToken);

        if (exists)
            throw new ConflictException($"A district with code '{req.Code}' already exists.");

        // CREACIÓN DDD: Tu mapper manual convierte el request inmutable en la entidad rica
        var district = req.ToEntity();

        _context.Districts.Add(district);
        await _context.SaveChangesAsync(cancellationToken);

        return district.ToResponse();
    }
}