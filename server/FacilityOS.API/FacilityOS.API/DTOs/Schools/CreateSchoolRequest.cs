using FacilityOS.API.Models;

namespace FacilityOS.API.DTOs.Schools;

public class CreateSchoolRequest
{
    public string Name { get; set; } = string.Empty;
    public string SchoolCode { get; set; } = string.Empty;
    public SchoolLevel Level { get; set; }
    public SchoolType Type { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public int StudentCapacity { get; set; }
    public int DistrictId { get; set; }
}