using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Auth;
using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FacilityOS.Application.Features.Auth.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenHandler(
        IApplicationDbContext context,
        IAuthService authService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var tokenToRefresh = command.RefreshToken;

        if (string.IsNullOrWhiteSpace(tokenToRefresh))
            throw new UnauthorizedAccessException("Refresh token is missing.");

        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == tokenToRefresh, cancellationToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        if (!storedToken.User.IsActive || storedToken.User.IsDeleted)
        {
            storedToken.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("User account is inactive or deleted.");
        }

        storedToken.Revoke();

        var newAccessToken = _authService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _authService.GenerateRefreshToken();

        var refreshTokenDays = _jwtSettings.RefreshTokenExpirationDays;

        var newRefreshToken = new Domain.Models.RefreshToken(
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(refreshTokenDays),
            storedToken.UserId
        );

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return storedToken.User.ToAuthResult(newAccessToken, newRefreshTokenValue);
    }
}