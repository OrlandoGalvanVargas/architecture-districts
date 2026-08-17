namespace FacilityOS.API.Models
{
    public enum UserEntityType
    {
        Global = 0,   // Súper Administrador o roles globales
        District = 1, // DistrictAdmin (EntityId = DistrictId)
        School = 2    // SchoolAdmin (EntityId = SchoolId)
    }
}
