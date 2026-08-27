using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetUserByIdHandler(IApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userResponse = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .ProjectToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        if (userResponse is null)
            throw new NotFoundException(nameof(User), request.Id);

        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == request.Id, cancellationToken);

        var canManage = await _authService.CanManageUserAsync(userEntity, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to view this user.");

        return userResponse;
    }
}
