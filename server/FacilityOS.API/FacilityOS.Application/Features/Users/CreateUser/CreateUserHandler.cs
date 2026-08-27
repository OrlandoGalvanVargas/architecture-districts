using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FacilityOS.Application.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly BCryptSettings _bCryptSettings;

    public CreateUserHandler(
        IApplicationDbContext context,
        IResourceAuthorizationService authService,
        IOptions<BCryptSettings> bCryptOptions)
    {
        _context = context;
        _authService = authService;
        _bCryptSettings = bCryptOptions.Value;
    }

    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var canCreate = await _authService.CanCreateUserRoleAsync(req.Role, req.EntityType, req.EntityId, cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenException("You do not have permission to create a user with these parameters.");
        }

        var entityExists = await _authService.ValidateEntityExistsAsync(req.EntityType, req.EntityId, cancellationToken);
        if (!entityExists)
        {
            throw new NotFoundException(req.EntityType.ToString(), req.EntityId ?? 0);
        }

        var exists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim(), cancellationToken);

        if (exists)
        {
            throw new ConflictException($"A user with email '{req.Email}' already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, _bCryptSettings.WorkFactor);

        var user = req.ToEntity(passwordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }
}