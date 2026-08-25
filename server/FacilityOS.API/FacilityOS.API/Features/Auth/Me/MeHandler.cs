using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Auth.Me;

public class MeHandler : IRequestHandler<MeQuery, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MeHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponse> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
            throw new UnauthorizedAccessException("User context could not be resolved.");

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user is null || !user.IsActive)
            throw new NotFoundException(nameof(User), userId.Value);

        return user.ToResponse();
    }
}
