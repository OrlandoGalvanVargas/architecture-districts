using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;

namespace FacilityOS.API.Common.Mapping;

public static class DistrictMapping
{
    public static DistrictResponse ToResponse(this District district)
    {
        return new DistrictResponse
        {
            Id = district.Id,
            Name = district.Name,
            Code = district.Code,
            State = district.State,
            City = district.City,
            ZipCode = district.ZipCode,
            Address = district.Address,
            Description = district.Description,
            SchoolCount = district.Schools?.Count ?? 0,
            BeaconCount = district.Beacons?.Count ?? 0,     
            FacultyCount = district.Faculties?.Count ?? 0,  
            CreatedAt = district.CreatedAt,
            UpdatedAt = district.UpdatedAt
        };
    }

    public static IQueryable<DistrictResponse> ProjectToResponse(this IQueryable<District> query)
    {
        return query.Select(d => new DistrictResponse
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            State = d.State,
            City = d.City,
            ZipCode = d.ZipCode,
            Address = d.Address,
            Description = d.Description,
            SchoolCount = d.Schools.Count(),
            BeaconCount = d.Beacons.Count(),     
            FacultyCount = d.Faculties.Count(),
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        });
    }

    public static District ToEntity(this CreateDistrictRequest request)
    {
        return new District(
            request.Name,
            request.Code,
            request.State,
            request.City,
            request.ZipCode,
            request.Address,
            request.Description
        );
    }
}
