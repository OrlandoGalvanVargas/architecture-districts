using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Models;

namespace FacilityOS.API.Common.Mapping;

public static class UserMapping
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            EntityId = user.EntityId,
            EntityType = user.EntityType,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static IQueryable<UserResponse> ProjectToResponse(this IQueryable<User> query)
    {
        return query.Select(u => new UserResponse
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
        });
    }

    public static User ToEntity(this CreateUserRequest request, string passwordHash)
    {
        var user = new User(
            request.Name,
            request.Email,
            passwordHash,
            request.Role
        );

        user.AssignToEntity(request.EntityId, request.EntityType);

        return user;
    }
}
