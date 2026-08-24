using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.DTOs.Users;

public record CreateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = "User";
    public int? EntityId { get; init; }
    public UserEntityType EntityType { get; init; } = UserEntityType.Global;
}

public record UpdateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int? EntityId { get; init; }
    public UserEntityType EntityType { get; init; } = UserEntityType.Global;
    public bool IsActive { get; init; } = true;
    public string? NewPassword { get; init; }
}

public record UserResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int? EntityId { get; init; }
    public UserEntityType EntityType { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}