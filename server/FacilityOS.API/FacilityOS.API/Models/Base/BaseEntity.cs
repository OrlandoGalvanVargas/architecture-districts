namespace FacilityOS.API.Models.Base;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; private set; } 
    public DateTime? UpdatedAt { get; private set; }
}