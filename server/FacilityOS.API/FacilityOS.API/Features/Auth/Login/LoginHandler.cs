using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public LoginHandler(
        ApplicationDbContext context, 
        IAuthService authService,
        IConfiguration configuration)
    {
        _context = context;
        _authService = authService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == req.Email.ToLower().Trim(), cancellationToken);

        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password, or account is disabled.");

        var accessToken = _authService.GenerateAccessToken(user);
        var refreshTokenValue = _authService.GenerateRefreshToken();
        
        var refreshTokenDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7);
        
        var refreshToken = new Models.RefreshToken(
            refreshTokenValue,
            DateTime.UtcNow.AddDays(refreshTokenDays),
            user.Id
        );

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToLoginResponse(accessToken, refreshTokenValue);
    }
}