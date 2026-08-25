using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.DTOs.Beacons;

public record CreateBeaconRequest
{
    public string DeviceName { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public BeaconType Type { get; init; }
    public int? DistrictId { get; init; }
    public int? SchoolId { get; init; }
}

public record UpdateBeaconRequest
{
    public string DeviceName { get; init; } = string.Empty;
    public BeaconType Type { get; init; }
    public BeaconStatus Status { get; init; }
    public int? DistrictId { get; init; }
    public int? SchoolId { get; init; }
}

public record BeaconResponse
{
    public int Id { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public BeaconType Type { get; init; }
    public BeaconStatus Status { get; init; }
    public int? DistrictId { get; init; }
    public string? DistrictName { get; init; }
    public int? SchoolId { get; init; }
    public string? SchoolName { get; init; }
    public int? FacultyId { get; init; }
    public string? FacultyName { get; init; }
    public bool IsAssigned => DistrictId.HasValue || SchoolId.HasValue || FacultyId.HasValue;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}