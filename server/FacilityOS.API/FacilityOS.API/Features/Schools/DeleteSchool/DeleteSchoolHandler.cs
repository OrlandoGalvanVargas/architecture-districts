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
        // A. SEGURIDAD (Multi-Tenancy): Validar si el usuario actual tiene permisos jerárquicos para gestionar esta escuela
        var canManage = await _authService.CanManageSchoolAsync(command.Id, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to delete this school.");

        // B. Recuperar la entidad viva de la base de datos (Aplica el filtro global IsDeleted = false automáticamente)
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), command.Id);

        // C. COMPORTAMIENTO DDD: Invocamos el método encapsulado de tu Rich Domain Model
        // Cambia las banderas de borrado lógico e inactivación protegiendo sus invariantes
        school.SoftDelete();

        // D. PERSISTENCIA ATÓMICA: Como alteramos propiedades, EF detecta el estado Modified.
        // Tu interceptor UpdateAuditableEntitiesInterceptor inyectará el UpdatedAt en milisegundos.
        await _context.SaveChangesAsync(cancellationToken);

        // No requiere return manual. MediatR y C# resuelven la finalización del Task automáticamente.
    }
}