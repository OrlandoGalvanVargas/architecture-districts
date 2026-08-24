using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales con proyección
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetUserByIdHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscamos y proyectamos el usuario directo en base de datos en un solo paso eficiente.
        // El query filter global de Soft Delete (IsDeleted == false) actúa de forma automática aquí.
        var userResponse = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .ProjectToResponse() // <- Proyección optimizada a nivel de base de datos SQL
            .FirstOrDefaultAsync(cancellationToken);

        // 2. Si no existe (o fue borrado lógicamente), disparamos un 404 inmediato (Fail-Fast)
        if (userResponse is null)
            throw new NotFoundException(nameof(User), request.Id);

        // 3. SEGURIDAD (Multi-Tenancy): Para validar los permisos jerárquicos necesitamos la entidad de Dominio.
        // Como ya sabemos que existe, la recuperamos de forma rápida.
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == request.Id, cancellationToken);

        var canManage = await _authService.CanManageUserAsync(userEntity, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to view this user.");

        // 4. Retornamos el DTO de respuesta limpio y pre-proyectado
        return userResponse;
    }
}
