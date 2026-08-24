using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales con proyección
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.GetSchoolById;

public class GetSchoolByIdHandler : IRequestHandler<GetSchoolByIdQuery, SchoolResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetSchoolByIdHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscamos y proyectamos la escuela directo en base de datos en un solo paso eficiente.
        // El query filter global de Soft Delete (IsDeleted == false) actúa de forma automática aquí.
        var schoolResponse = await _context.Schools
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .ProjectToResponse() // <- Reutilización de tu mapeo manual nativo
            .FirstOrDefaultAsync(cancellationToken);

        // 2. Si no existe en la base de datos (o fue borrada por soft delete), disparamos un 404 inmediato
        if (schoolResponse is null)
            throw new NotFoundException(nameof(School), request.Id);

        // 3. Si existe, validamos si el usuario actual tiene permisos jerárquicos/contextuales para verla
        var canAccess = await _authService.CanAccessSchoolAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this school.");

        // 4. Retornamos la respuesta limpia y estandarizada
        return schoolResponse;
    }
}
