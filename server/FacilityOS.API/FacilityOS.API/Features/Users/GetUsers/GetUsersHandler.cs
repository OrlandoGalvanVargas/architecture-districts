using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Features.Schools.GetSchools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Users.GetUsers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserResponse>>
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUsersHandler(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (_currentUser.IsDistrictAdmin)
        {
            var schoolIds = await _context.Schools
                .Where(s => s.DistrictId == _currentUser.EntityId)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(u =>
                (u.EntityType == UserEntityType.District && u.EntityId == _currentUser.EntityId) ||
                (u.EntityType == UserEntityType.School && u.EntityId.HasValue && schoolIds.Contains(u.EntityId.Value)));
        }
        else if (_currentUser.IsSchoolAdmin)
        {
            query = query.Where(u => u.EntityType == UserEntityType.School && u.EntityId == _currentUser.EntityId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(searchTerm) ||
                                     u.Email.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
            query = query.Where(u => u.Role == request.Role);

        if (request.EntityType.HasValue)
            query = query.Where(u => u.EntityType == request.EntityType.Value);

        if (request.EntityId.HasValue)
            query = query.Where(u => u.EntityId == request.EntityId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                EntityId = u.EntityId,
                EntityType = u.EntityType,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}