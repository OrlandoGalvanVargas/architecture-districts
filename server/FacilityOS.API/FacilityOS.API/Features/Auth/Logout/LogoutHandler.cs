using FacilityOS.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public LogoutHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == command.RefreshToken, cancellationToken);

        if (token is null)
            return false;

        token.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}