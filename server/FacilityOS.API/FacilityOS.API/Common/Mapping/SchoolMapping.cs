using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;

namespace FacilityOS.API.Common.Mapping;

public static class SchoolMapping
{
    public static SchoolResponse ToResponse(this School school)
    {
        return new SchoolResponse
        {
            Id = school.Id,
            Name = school.Name,
            SchoolCode = school.SchoolCode,
            Level = school.Level,
            Type = school.Type,
            Address = school.Address,
            City = school.City,
            State = school.State,
            ZipCode = school.ZipCode,
            Phone = school.Phone,
            ContactEmail = school.ContactEmail,
            StudentCapacity = school.StudentCapacity,
            IsActive = school.IsActive,
            DistrictId = school.DistrictId,
            DistrictName = school.District?.Name ?? string.Empty,
            BeaconCount = school.Beacons?.Count ?? 0,    
            FacultyCount = school.Faculties?.Count ?? 0,  
            CreatedAt = school.CreatedAt,
            UpdatedAt = school.UpdatedAt
        };
    }

    public static IQueryable<SchoolResponse> ProjectToResponse(this IQueryable<School> query)
    {
        return query.Select(s => new SchoolResponse
        {
            Id = s.Id,
            Name = s.Name,
            SchoolCode = s.SchoolCode,
            Level = s.Level,
            Type = s.Type,
            Address = s.Address,
            City = s.City,
            State = s.State,
            ZipCode = s.ZipCode,
            Phone = s.Phone,
            ContactEmail = s.ContactEmail,
            StudentCapacity = s.StudentCapacity,
            IsActive = s.IsActive,
            DistrictId = s.DistrictId,
            DistrictName = s.District.Name,
            BeaconCount = s.Beacons.Count(),   
            FacultyCount = s.Faculties.Count(),  
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        });
    }

    public static School ToEntity(this CreateSchoolRequest request)
    {
        return new School(
            request.Name,
            request.SchoolCode,
            request.Level,
            request.Type,
            request.Address,
            request.City,
            request.State,
            request.ZipCode,
            request.DistrictId,
            request.StudentCapacity,
            request.Phone,
            request.ContactEmail
        );
    }
}