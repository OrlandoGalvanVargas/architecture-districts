using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales con proyección IQueryable
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Features.Schools.GetSchools; // Mantenemos la referencia temporal a tu PagedResult
using FacilityOS.API.Models.Enums;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.GetUsers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUsersHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // 1. Iniciamos la consulta base sin tracking (El filtro global IsDeleted = false actúa de forma invisible)
        var query = _context.Users.AsNoTracking().AsQueryable();

        // 2. CAPA DE SEGURIDAD (Multi-Tenancy): Restringir el universo según el rol del usuario conectado
        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;

            // Enfoque experto: Dejamos la consulta de escuelas como IQueryable (SIN materializar con ToList)
            // Esto permite que EF Core fusione todo en una sola Query nativa y eficiente en SQL Server.
            var schoolIdsQuery = _context.Schools
                .Where(s => s.DistrictId == districtId)
                .Select(s => s.Id);

            query = query.Where(u =>
                (u.EntityType == UserEntityType.District && u.EntityId == districtId) ||
                (u.EntityType == UserEntityType.School && u.EntityId.HasValue && schoolIdsQuery.Contains(u.EntityId.Value)));
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            query = query.Where(u => u.EntityType == UserEntityType.School && u.EntityId == _currentUser.EntityId.Value);
        }

        // 3. CAPA DE FILTROS DINÁMICOS: Parámetros opcionales del cliente
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(searchTerm) ||
                                     u.Email.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            query = query.Where(u => u.Role == request.Role);
        }

        if (request.EntityType.HasValue)
        {
            query = query.Where(u => u.EntityType == request.EntityType.Value);
        }

        if (request.EntityId.HasValue)
        {
            query = query.Where(u => u.EntityId == request.EntityId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        // 4. Conteo total en base de datos antes de paginar
        var totalCount = await query.CountAsync(cancellationToken);

        // 5. Ordenación, Paginación, Proyección SQL Eficiente y Materialización
        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToResponse() // <- Reutilización de tu mapeo manual (Elimina el Select procedural)
            .ToListAsync(cancellationToken);

        // 6. Retorno estructurado
        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
