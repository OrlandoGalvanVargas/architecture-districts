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

    public RefreshTokenHandler(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Validar estado de la cuenta del usuario
        if (storedToken.User.IsDeleted || !storedToken.User.IsActive)
        {
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("User account is inactive or deleted.");
        }

        storedToken.IsRevoked = true;

        var newAccessToken = _authService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _authService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new Models.RefreshToken
        {
            Token = newRefreshTokenValue,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            User = new UserDto
            {
                Id = storedToken.User.Id,
                Name = storedToken.User.Name,
                Email = storedToken.User.Email,
                Role = storedToken.User.Role,
                EntityId = storedToken.User.EntityId,
                EntityType = storedToken.User.EntityType.ToString()
            }
        };
    }
}