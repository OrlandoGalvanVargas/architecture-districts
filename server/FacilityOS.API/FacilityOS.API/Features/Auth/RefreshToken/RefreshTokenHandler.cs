using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Auth;
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

        var refreshTokenDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7);

        var newRefreshToken = new Models.RefreshToken(
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(refreshTokenDays),
            storedToken.UserId
        );

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return storedToken.User.ToLoginResponse(newAccessToken, newRefreshTokenValue);
    }
}