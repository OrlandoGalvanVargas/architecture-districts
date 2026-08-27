
using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Domain.Models.Enums;
using MediatR;

namespace FacilityOS.Application.Features.Users.GetUsers;

public record GetUsersQuery(
    string? Search,
    string? Role,
    UserEntityType? EntityType,
    int? EntityId,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<PagedResult<UserResponse>>;