using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(
        ApplicationDbContext context,
        IAuthService authService,
        IConfiguration configuration)
    {
        _context = context;
        _authService = authService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        // Validar token
        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Validar estado de la cuenta del usuario
        if (!storedToken.User.IsActive || storedToken.User.IsDeleted)
        {
            storedToken.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("User account is inactive or deleted.");
        }

        // Revocar token actual
        storedToken.Revoke();

        // Generar nuevos tokens
        var newAccessToken = _authService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _authService.GenerateRefreshToken();

        // Obtener expiración del refresh token desde configuración
        var refreshTokenDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7);

        // Crear nuevo refresh token usando constructor
        var newRefreshToken = new Models.RefreshToken(
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(refreshTokenDays),
            storedToken.UserId
        );

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Usar el mapping centralizado
        return storedToken.User.ToLoginResponse(newAccessToken, newRefreshTokenValue);
    }
}