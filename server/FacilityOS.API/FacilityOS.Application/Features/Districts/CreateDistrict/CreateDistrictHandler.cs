using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Districts;
using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Districts.CreateDistrict;

public class CreateDistrictHandler : IRequestHandler<CreateDistrictCommand, DistrictResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateDistrictHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DistrictResponse> Handle(CreateDistrictCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can create new districts.");

        var req = command.Request;

        var exists = await _context.Districts
            .AnyAsync(d => d.Code.ToLower() == req.Code.ToLower().Trim(), cancellationToken);

        if (exists)
            throw new ConflictException($"A district with code '{req.Code}' already exists.");

        var district = req.ToEntity();

        _context.Districts.Add(district);
        await _context.SaveChangesAsync(cancellationToken);

        return district.ToResponse();
    }
}