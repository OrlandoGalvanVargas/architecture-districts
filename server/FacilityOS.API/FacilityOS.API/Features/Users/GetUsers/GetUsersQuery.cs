
using FacilityOS.API.Common;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models.Enums;
using MediatR;

namespace FacilityOS.API.Features.Users.GetUsers;

public record GetUsersQuery(
    string? Search,
    string? Role,
    UserEntityType? EntityType,
    int? EntityId,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<PagedResult<UserResponse>>;