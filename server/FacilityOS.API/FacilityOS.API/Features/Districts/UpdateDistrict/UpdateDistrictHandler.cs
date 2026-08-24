using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.UpdateDistrict;

public class UpdateDistrictHandler : IRequestHandler<UpdateDistrictCommand, DistrictResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public UpdateDistrictHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<DistrictResponse> Handle(UpdateDistrictCommand command, CancellationToken cancellationToken)
    {
        // 1. Multi-tenancy: Validar acceso usando el command.Id
        var canAccess = await _authService.CanAccessDistrictAsync(command.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to modify this district.");

        // 2. Extraemos el request para mayor comodidad de lectura
        var req = command.Request;

        // 3. Recuperar la entidad viva con sus escuelas para el conteo automático del mapper
        var district = await _context.Districts
            .Include(d => d.Schools)
            .FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), command.Id);

        // 4. Validar duplicados de código
        var codeTaken = await _context.Districts
            .AnyAsync(d => d.Code.ToLower() == req.Code.ToLower().Trim() && d.Id != command.Id, cancellationToken);

        if (codeTaken)
            throw new ConflictException($"A district with the code '{req.Code}' already exists.");

        // 5. MUTACIÓN DDD: Invocamos el método encapsulado de la clase de dominio
        district.Update(
            req.Name.Trim(),
            req.Code.ToUpper().Trim(),
            req.State.ToUpper().Trim(),
            req.City.Trim(),
            req.ZipCode.Trim(),
            req.Address.Trim(),
            req.Description?.Trim()
        );

        // 6. Guardar cambios. El interceptor inyecta el UpdatedAt automáticamente
        await _context.SaveChangesAsync(cancellationToken);

        // 7. Retornar respuesta usando el mapper manual ToResponse()
        return district.ToResponse();
    }
}
