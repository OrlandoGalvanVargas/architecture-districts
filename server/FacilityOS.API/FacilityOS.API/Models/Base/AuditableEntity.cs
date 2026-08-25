namespace FacilityOS.API.Models.Base;

public abstract class AuditableEntity : BaseEntity
{
    public bool IsActive { get; protected set; } = true;
    public bool IsDeleted { get; protected set; } = false;

    public void SoftDelete()
    {
        IsDeleted = true;
        IsActive = false;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}