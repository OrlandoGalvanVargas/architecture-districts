using FacilityOS.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly ApplicationDbContext _context;

    public LogoutHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
            var utcNow = DateTime.UtcNow;

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => 
                    t.Token == command.RefreshToken && 
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