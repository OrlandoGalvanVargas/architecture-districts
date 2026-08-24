using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.DeleteDistrict;

public class DeleteDistrictHandler : IRequestHandler<DeleteDistrictCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteDistrictHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteDistrictCommand command, CancellationToken cancellationToken)
    {
        // A. Validación de Seguridad de Rol Global
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Only global administrators can delete districts.");

        // B. Recuperar la entidad viva de la base de datos (Aplica el filtro IsDeleted automáticamente)
        var district = await _context.Districts
            .FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), command.Id);

        // C. BLINDAJE DE INFRAESTRUCTURA: Validar escuelas asociadas (Activas y en Soft Delete)
        // Usamos .IgnoreQueryFilters() para que SQL Server busque TODO el universo físico de registros.
        // Esto evita que salte la restricción DeleteBehavior.Restrict de la base de datos de forma descontrolada.
        var hasSchools = await _context.Schools
            .IgnoreQueryFilters()
            .AnyAsync(s => s.DistrictId == command.Id, cancellationToken);

        if (hasSchools)
        {
            throw new ConflictException("Cannot delete a district that contains schools (active or archived). Reassign or purge schools first.");
        }

        // D. Ejecutar la remoción física de la base de datos
        _context.Districts.Remove(district);
        await _context.SaveChangesAsync(cancellationToken);

        // Cero retornos manuales. ¡MediatR maneja la tarea completada sola!
    }
}