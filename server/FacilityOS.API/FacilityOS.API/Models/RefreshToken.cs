using FacilityOS.API.Models.Base;

namespace FacilityOS.API.Models;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public RefreshToken(string token, DateTime expiresAt, int userId)
    {
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
    }

    public void Revoke()
    {
        if (IsRevoked) return;

        IsRevoked = true;
    }
}