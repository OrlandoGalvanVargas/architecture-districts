using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Faculties.DeleteFaculty;

public class DeleteFacultyCommandHandler : IRequestHandler<DeleteFacultyCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public DeleteFacultyCommandHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task Handle(DeleteFacultyCommand command, CancellationToken cancellationToken)
    {
        var canManage = await _authService.CanManageFacultyAsync(command.Id, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to delete this faculty.");

        var faculty = await _context.Faculties
            .Include(f => f.Beacon) 
            .FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (faculty is null)
            throw new NotFoundException(nameof(Faculty), command.Id);

        if (faculty.Beacon is not null)
        {
            faculty.Beacon.Unassign();
        }

        faculty.SoftDelete();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
