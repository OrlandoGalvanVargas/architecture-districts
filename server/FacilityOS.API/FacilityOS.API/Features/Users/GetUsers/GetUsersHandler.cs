using FacilityOS.API.Common;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models.Enums;
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
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (_currentUser.IsDistrictAdmin && _currentUser.EntityId.HasValue)
        {
            var districtId = _currentUser.EntityId.Value;

            var schoolIdsQuery = _context.Schools
                .Where(s => s.DistrictId == districtId)
                .Select(s => s.Id);

            query = query.Where(u =>
                (u.EntityType == UserEntityType.District && u.EntityId == districtId) ||
                (u.EntityType == UserEntityType.School && u.EntityId.HasValue && schoolIdsQuery.Contains(u.EntityId.Value)));
        }
        else if (_currentUser.IsSchoolAdmin && _currentUser.EntityId.HasValue)
        {
            query = query.Where(u => u.EntityType == UserEntityType.School && u.EntityId == _currentUser.EntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(searchTerm) ||
                                     u.Email.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            query = query.Where(u => u.Role == request.Role);
        }

        if (request.EntityType.HasValue)
        {
            query = query.Where(u => u.EntityType == request.EntityType.Value);
        }

        if (request.EntityId.HasValue)
        {
            query = query.Where(u => u.EntityId == request.EntityId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToResponse() 
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
