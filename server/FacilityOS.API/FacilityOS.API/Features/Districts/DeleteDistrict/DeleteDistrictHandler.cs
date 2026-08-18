using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.DeleteDistrict;

public class DeleteDistrictHandler : IRequestHandler<DeleteDistrictCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteDistrictHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteDistrictCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can delete districts.");

        var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), command.Id);

        var hasSchools = await _context.Schools.AnyAsync(s => s.DistrictId == command.Id, cancellationToken);
        if (hasSchools)
            throw new ConflictException("Cannot delete a district that contains active schools. Reassign or delete schools first.");

        _context.Districts.Remove(district);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}