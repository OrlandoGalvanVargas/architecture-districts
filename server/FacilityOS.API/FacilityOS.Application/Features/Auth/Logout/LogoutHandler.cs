using FacilityOS.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;

    public LogoutHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var tokenToRevoke = command.RefreshToken;

        if (!string.IsNullOrWhiteSpace(tokenToRevoke))
        {
            var utcNow = DateTime.UtcNow;
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t =>
                    t.Token == tokenToRevoke &&
                    !t.IsRevoked &&
                    t.ExpiresAt > utcNow,
                    cancellationToken);

            if (token is not null)
            {
                token.Revoke();
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}