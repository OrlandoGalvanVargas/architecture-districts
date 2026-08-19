using FacilityOS.API.Models;

namespace FacilityOS.API.DTOs.Users;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public UserEntityType EntityType { get; set; } = UserEntityType.Global;
}