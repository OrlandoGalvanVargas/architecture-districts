using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
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

        // 1. CAPA DE SEGURIDAD (Multi-Tenancy): Validar si el usuario actual tiene permisos jerárquicos para asignar este rol y alcance
        var canCreate = await _authService.CanCreateUserRoleAsync(req.Role, req.EntityType, req.EntityId, cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenException("You do not have permission to create a user with these parameters.");
        }

        // 2. REGLA DE NEGOCIO: Validar la existencia física y lógica de la entidad vinculada (District o School)
        var entityExists = await _authService.ValidateEntityExistsAsync(req.EntityType, req.EntityId, cancellationToken);
        if (!entityExists)
        {
            throw new NotFoundException(req.EntityType.ToString(), req.EntityId ?? 0);
        }

        // 3. REGLA DE NEGOCIO: Validar colisiones de correos únicos en el storage (Aprovecha automáticamente el Query Filter global)
        var exists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim(), cancellationToken);

        if (exists)
        {
            throw new ConflictException($"A user with email '{req.Email}' already exists.");
        }

        // 4. INFRAESTRUCTURA: Hashing criptográfico de la contraseña
        var workFactor = _configuration.GetValue<int>("BCrypt:WorkFactor", 12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor);

        // 5. CREACIÓN DDD: Tu mapper manual convierte el record inmutable en la entidad rica.
        // Internamente ejecuta el constructor base y llama a user.AssignToEntity(...) de forma encapsulada.
        var user = req.ToEntity(passwordHash);

        // 6. PERSISTENCIA ATÓMICA: Guardamos en base de datos. El interceptor gestiona el 'CreatedAt' transparente.
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 7. RESPUESTA: Mapeo nativo ultra rápido directo a la salida de la API
        return user.ToResponse();
    }
}