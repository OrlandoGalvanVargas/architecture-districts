using FacilityOS.API.Models.Base;
using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.Models;

public class User : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "User";
    public int? EntityId { get; private set; }
    public UserEntityType EntityType { get; private set; } = UserEntityType.Global;
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User() { }

    public User(string name, string email, string passwordHash, string role = "User")
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AssignToEntity(int? entityId, UserEntityType entityType)
    {
        EntityId = entityId;
        EntityType = entityType;
    }

    public void UpdateRole(string role)
    {
        Role = role;
    }
}