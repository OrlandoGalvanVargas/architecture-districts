using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Auth;
using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FacilityOS.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public LoginHandler(
        IApplicationDbContext context,
        IAuthService authService,
        IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _authService = authService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim(), cancellationToken);

        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password, or account is disabled.");

        var accessToken = _authService.GenerateAccessToken(user);
        var refreshTokenValue = _authService.GenerateRefreshToken();

        var refreshTokenDays = _jwtSettings.RefreshTokenExpirationDays;

        var refreshToken = new Domain.Models.RefreshToken(
            refreshTokenValue,
            DateTime.UtcNow.AddDays(refreshTokenDays),
            user.Id
        );

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToAuthResult(accessToken, refreshTokenValue);
    }
}