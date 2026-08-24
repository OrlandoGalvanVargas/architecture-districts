using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserHandler(
        ApplicationDbContext context,
        IResourceAuthorizationService authService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // A. REGLA DE NEGOCIO CRÍTICA: Impedir el autoborrado de cuentas
        if (_currentUser.UserId == command.Id)
            throw new InvalidOperationException("You cannot delete your own account.");

        // B. PERSISTENCIA: Recuperar la entidad viva (El filtro global IsDeleted actúa automáticamente)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(User), command.Id);

        // C. CAPA DE SEGURIDAD (Multi-Tenancy): Validar si el operador tiene jerarquía sobre el usuario objetivo
        var canManage = await _authService.CanManageUserAsync(user, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to delete this user.");

        // D. COMPORTAMIENTO DDD: Invocamos el método encapsulado de tu Rich Domain Model
        // Cambia las banderas de borrado lógico e inactivación protegiendo sus invariantes
        user.SoftDelete();

        // E. SEGURIDAD DE INFRAESTRUCTURA: Purgar atómicamente todas sus sesiones activas (Refresh Tokens)
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == command.Id)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(activeTokens);

        // F. PERSISTENCIA ATÓMICA: El interceptor inyectará automáticamente el UpdatedAt
        await _context.SaveChangesAsync(cancellationToken);

        // Cero retornos manuales. MediatR y C# resuelven la finalización de la tarea.
    }
}