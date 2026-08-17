using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public LoginHandler(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email, cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            var accessToken = _authService.GenerateAccessToken(user);
            var refreshTokenValue = _authService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new Models.RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    EntityId = user.EntityId,
                    EntityType = user.EntityType
                }
            };
        }
    }
}
