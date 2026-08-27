using FacilityOS.Domain.Models.Enums;

namespace FacilityOS.Application.DTOs.Faculties;

public record CreateFacultyRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Title { get; init; }
    public string? Department { get; init; }
    public int? DistrictId { get; init; }
    public int? SchoolId { get; init; }
    public int? BeaconId { get; init; }
}

public record UpdateFacultyRequest : CreateFacultyRequest
{
    public bool IsActive { get; init; } = true;

}

public record FacultyResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Title { get; init; }
    public string? Department { get; init; }
    public int? DistrictId { get; init; }
    public string? DistrictName { get; init; }
    public int? SchoolId { get; init; }
    public string? SchoolName { get; init; }
    public int? BeaconId { get; init; }
    public string? BeaconDeviceName { get; init; }
    public string? BeaconSerialNumber { get; init; }
    public BeaconType? BeaconType { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}