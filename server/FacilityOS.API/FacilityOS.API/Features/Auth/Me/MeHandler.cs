using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Me;

public class MeHandler : IRequestHandler<MeQuery, UserDto>
{
    private readonly ApplicationDbContext _context;

    public MeHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted && u.IsActive, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(User), request.UserId);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            EntityId = user.EntityId,
            EntityType = user.EntityType.ToString()
        };
    }
}