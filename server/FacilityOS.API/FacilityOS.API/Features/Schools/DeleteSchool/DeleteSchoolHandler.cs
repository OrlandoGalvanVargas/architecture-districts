using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.DeleteSchool;

public class DeleteSchoolHandler : IRequestHandler<DeleteSchoolCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public DeleteSchoolHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task Handle(DeleteSchoolCommand command, CancellationToken cancellationToken)
    {
        var canManage = await _authService.CanManageSchoolAsync(command.Id, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to delete this school.");

        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), command.Id);

        school.SoftDelete();

        await _context.SaveChangesAsync(cancellationToken);
    }
}