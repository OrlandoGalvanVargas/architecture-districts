using FacilityOS.Domain.Models.Enums;

namespace FacilityOS.Application.DTOs.Schools;

public record CreateSchoolRequest
{
    public string Name { get; init; } = string.Empty;
    public string SchoolCode { get; init; } = string.Empty;
    public SchoolLevel Level { get; init; }
    public SchoolType Type { get; init; }
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? ContactEmail { get; init; }
    public int StudentCapacity { get; init; }
    public int DistrictId { get; init; }
}

public record UpdateSchoolRequest : CreateSchoolRequest
{
    public bool IsActive { get; init; } = true;
}

public record SchoolResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SchoolCode { get; init; } = string.Empty;
    public SchoolLevel Level { get; init; }
    public SchoolType Type { get; init; }
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? ContactEmail { get; init; }
    public int StudentCapacity { get; init; }
    public bool IsActive { get; init; }
    public int DistrictId { get; init; }
    public string DistrictName { get; init; } = string.Empty;
    public int BeaconCount { get; init; }
    public int FacultyCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}