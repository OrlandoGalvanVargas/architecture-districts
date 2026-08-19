using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly IConfiguration _configuration;

    public CreateUserHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService,
        IConfiguration configuration)
    {
        _context = context;
        _authService = authService;
        _configuration = configuration;
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
            .AnyAsync(u => u.Email.ToLower() == req.Email.ToLower(), cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"A user with email '{req.Email}' already exists.");
        }

        var workFactor = _configuration.GetValue<int>("BCrypt:WorkFactor", 12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor);

        var user = new User
        {
            Name = req.Name.Trim(),
            Email = req.Email.ToLower().Trim(),
            PasswordHash = passwordHash,
            Role = req.Role,
            EntityId = req.EntityId,
            EntityType = req.EntityType,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            EntityId = user.EntityId,
            EntityType = user.EntityType,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}