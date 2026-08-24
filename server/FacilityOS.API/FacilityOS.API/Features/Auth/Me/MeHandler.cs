using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;
using FacilityOS.API.Services; // Para consumir tu ICurrentUserService
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Me;

public class MeHandler : IRequestHandler<MeQuery, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MeHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponse> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        // Recuperamos el ID directamente del token validado a través de tu servicio experto
        var userId = _currentUserService.UserId;

        if (userId is null)
            throw new UnauthorizedAccessException("User context could not be resolved.");

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        // Si el usuario fue desactivado o eliminado por soft delete, lanzamos un 404 limpio
        if (user is null || !user.IsActive)
            throw new NotFoundException(nameof(User), userId.Value);

        // Mapeo manual optimizado nativo
        return user.ToResponse();
    }
}
