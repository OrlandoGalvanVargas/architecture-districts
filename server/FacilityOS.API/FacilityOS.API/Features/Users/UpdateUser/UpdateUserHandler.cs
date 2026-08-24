using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales estáticos
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly IConfiguration _configuration;

    public UpdateUserHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService,
        IConfiguration configuration)
    {
        _context = context;
        _authService = authService;
        _configuration = configuration;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        // A. Recuperar la entidad viva de la base de datos (El filtro global IsDeleted actúa automáticamente)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(User), command.Id);

        // B. CAPA DE SEGURIDAD: Validar si el usuario actual tiene permisos contextuales para gestionar a este usuario objetivo
        var canManage = await _authService.CanManageUserAsync(user, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to modify this user.");

        var req = command.Request;

        // C. REGLA DE NEGOCIO: Si cambian roles o alcances, validar que el operador tenga jerarquía suficiente (Asíncrono)
        if (user.Role != req.Role || user.EntityType != req.EntityType || user.EntityId != req.EntityId)
        {
            var canAssign = await _authService.CanCreateUserRoleAsync(req.Role, req.EntityType, req.EntityId, cancellationToken);
            if (!canAssign)
                throw new ForbiddenException("You do not have permission to assign these roles or scopes.");
        }

        // D. REGLA DE NEGOCIO: Validar colisiones de correos duplicados
        if (!user.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim() && u.Id != command.Id, cancellationToken);

            if (emailTaken)
                throw new ConflictException($"A user with email '{req.Email}' already exists.");
        }

        // E. CÁLCULO DE SEGURIDAD: Determinar si se debe purgar la sesión del usuario modificado
        bool securityChanged = !req.IsActive ||
                              !string.IsNullOrWhiteSpace(req.NewPassword) ||
                              user.Role != req.Role ||
                              user.EntityType != req.EntityType ||
                              user.EntityId != req.EntityId;

        // F. MUTACIÓN DDD: Invocamos los comportamientos explícitos de tu modelo de dominio rico
        user.Update(req.Name.Trim(), req.Email.ToLower().Trim());
        user.UpdateRole(req.Role);
        user.AssignToEntity(req.EntityId, req.EntityType);

        // Control de activación/desactivación utilizando tus métodos de negocio de AuditableEntity
        if (req.IsActive != user.IsActive)
        {
            if (req.IsActive)
                user.Activate();
            else
                user.Deactivate();
        }
        // Actualización criptográfica de contraseña si fue enviada
        if (!string.IsNullOrWhiteSpace(req.NewPassword))
        {
            var workFactor = _configuration.GetValue<int>("BCrypt:WorkFactor", 12);
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword, workFactor);
            user.UpdatePassword(newPasswordHash);
        }

        // G. INFRAESTRUCTURA DE SEGURIDAD: Revocación atómica de Refresh Tokens activos si hubo cambios críticos
        if (securityChanged)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id)
                .ToListAsync(cancellationToken);

            _context.RefreshTokens.RemoveRange(activeTokens);
        }

        // H. PERSISTENCIA ATÓMICA: El interceptor inyecta el UpdatedAt automáticamente al detectar el estado Modified
        await _context.SaveChangesAsync(cancellationToken);

        // I. RESPUESTA: Mapeo manual estático de rendimiento nativo libre de asignaciones procedurales
        return user.ToResponse();
    }
}
