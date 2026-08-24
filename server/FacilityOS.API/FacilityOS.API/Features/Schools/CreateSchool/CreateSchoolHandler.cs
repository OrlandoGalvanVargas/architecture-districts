using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; // Importamos tus mappers manuales ricos
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.CreateSchool;

public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand, SchoolResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public CreateSchoolHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        // A. CAPA DE SEGURIDAD: Validación Multi-Tenancy usando el método asíncrono corregido
        var canCreate = await _authService.CanCreateSchoolInDistrictAsync(req.DistrictId, cancellationToken);
        if (!canCreate)
            throw new ForbiddenException("You do not have permission to create a school in this district.");

        // B. REGLA DE NEGOCIO: Validar existencia del distrito y cargarlo en memoria para optimizar el mapa de salida
        // El query filter inyecta automáticamente IsDeleted = false aquí
        var district = await _context.Districts
            .FirstOrDefaultAsync(d => d.Id == req.DistrictId, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), req.DistrictId);

        // C. REGLA DE NEGOCIO: Validar duplicados de códigos de escuela
        var codeExists = await _context.Schools
            .AnyAsync(s => s.SchoolCode.ToLower() == req.SchoolCode.ToLower().Trim(), cancellationToken);

        if (codeExists)
            throw new ConflictException($"A school with code '{req.SchoolCode}' already exists.");

        // D. CREACIÓN DDD: El DTO inmutable se transforma en la entidad rica invocando su constructor encapsulado
        var school = req.ToEntity();

        // E. PERSISTENCIA ATÓMICA: Guardamos en base de datos. El Interceptor gestiona el 'CreatedAt' transparente
        _context.Schools.Add(school);
        await _context.SaveChangesAsync(cancellationToken);

        // F. RESPUESTA: Usamos tu mapper manual ToResponse().
        // Como EF Core ya conoce el objeto 'district' en memoria, la propiedad de navegación 'school.District'
        // se resuelve automáticamente sin necesidad de disparar la ineficiente query extra de LoadAsync().
        return school.ToResponse();
    }
}
