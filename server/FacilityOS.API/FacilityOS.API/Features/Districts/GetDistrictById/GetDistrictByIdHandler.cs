using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistrictById;

public class GetDistrictByIdHandler : IRequestHandler<GetDistrictByIdQuery, DistrictResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetDistrictByIdHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<DistrictResponse> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscamos y proyectamos el distrito directamente a nivel SQL en un solo paso eficiente
        // El filtro de Soft Delete actúa automáticamente aquí.
        var districtResponse = await _context.Districts
            .AsNoTracking()
            .Where(d => d.Id == request.Id)
            .ProjectToResponse() // <- Reutilización de tu mapeo manual nativo
            .FirstOrDefaultAsync(cancellationToken);

        // 2. Si no existe en la base de datos, disparamos un 404 inmediato (Falla rápido)
        if (districtResponse is null)
            throw new NotFoundException(nameof(District), request.Id);

        // 3. Si existe, validamos si el usuario actual tiene permisos contextuales para verlo
        var canAccess = await _authService.CanAccessDistrictAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this district.");

        // 4. Retornamos la respuesta estandarizada
        return districtResponse;
    }
}
