using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Auth.Me;

public class MeHandler : IRequestHandler<MeQuery, UserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MeHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
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
