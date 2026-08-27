using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserHandler(
        IApplicationDbContext context,
        IResourceAuthorizationService authService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == command.Id)
            throw new InvalidOperationException("You cannot delete your own account.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(User), command.Id);

        var canManage = await _authService.CanManageUserAsync(user, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to delete this user.");

        user.SoftDelete();

        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == command.Id)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(activeTokens);

        await _context.SaveChangesAsync(cancellationToken);
    }
}