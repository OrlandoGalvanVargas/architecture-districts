using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales optimizados
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.GetDistricts;

public class GetDistrictsHandler : IRequestHandler<GetDistrictsQuery, List<DistrictResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDistrictsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<DistrictResponse>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        // 1. Iniciamos la consulta base sin tracking para máxima velocidad de lectura
        // El filtro de Soft Delete (IsDeleted == false) se inyecta automáticamente aquí por EF Core
        var query = _context.Districts.AsNoTracking().AsQueryable();

        // 2. Aplicamos filtros de seguridad Multi-Tenancy basados en el alcance del usuario
        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;
            query = query.Where(d => d.Id == districtId);
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            var schoolId = _currentUser.EntityId.Value;
            query = query.Where(d => d.Schools.Any(s => s.Id == schoolId));
        }
        // Nota Senior: Si es Admin Global, no entra a ningún IF y lista absolutamente todo de forma correcta.

        // 3. Ordenamos, proyectamos eficientemente a nivel SQL y materializamos la lista
        return await query
            .OrderBy(d => d.Name)
            .ProjectToResponse() // <- Reutilización de tu joya de mapeo manual
            .ToListAsync(cancellationToken);
    }
}
