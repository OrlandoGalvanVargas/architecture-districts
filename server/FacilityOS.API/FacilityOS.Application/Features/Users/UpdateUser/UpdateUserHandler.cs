using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FacilityOS.Application.Features.Users.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly BCryptSettings _bCryptSettings;

    public UpdateUserHandler(
        IApplicationDbContext context,
        IResourceAuthorizationService authService,
        IOptions<BCryptSettings> bCryptOptions)
    {
        _context = context;
        _authService = authService;
        _bCryptSettings = bCryptOptions.Value;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(User), command.Id);

        var canManage = await _authService.CanManageUserAsync(user, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to modify this user.");

        var req = command.Request;

        if (user.Role != req.Role || user.EntityType != req.EntityType || user.EntityId != req.EntityId)
        {
            var canAssign = await _authService.CanCreateUserRoleAsync(req.Role, req.EntityType, req.EntityId, cancellationToken);
            if (!canAssign)
                throw new ForbiddenException("You do not have permission to assign these roles or scopes.");
        }

        if (user.EntityType != req.EntityType || user.EntityId != req.EntityId)
        {
            var entityExists = await _authService.ValidateEntityExistsAsync(req.EntityType, req.EntityId, cancellationToken);
            if (!entityExists)
                throw new NotFoundException(req.EntityType.ToString(), req.EntityId ?? 0);
        }

        if (!user.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim() && u.Id != command.Id, cancellationToken);

            if (emailTaken)
                throw new ConflictException($"A user with email '{req.Email}' already exists.");
        }

        bool securityChanged = !req.IsActive ||
                              !string.IsNullOrWhiteSpace(req.NewPassword) ||
                              user.Role != req.Role ||
                              user.EntityType != req.EntityType ||
                              user.EntityId != req.EntityId;

        user.Update(req.Name.Trim(), req.Email.ToLower().Trim());
        user.UpdateRole(req.Role);
        user.AssignToEntity(req.EntityId, req.EntityType);

        if (req.IsActive != user.IsActive)
        {
            if (req.IsActive)
                user.Activate();
            else
                user.Deactivate();
        }

        if (!string.IsNullOrWhiteSpace(req.NewPassword))
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword, _bCryptSettings.WorkFactor);
            user.UpdatePassword(newPasswordHash);
        }

        if (securityChanged)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }
}