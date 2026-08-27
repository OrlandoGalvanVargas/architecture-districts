using FacilityOS.Application.DTOs.Faculties;
using FacilityOS.Domain.Models;
using FacilityOS.Domain.Models.Enums;


namespace FacilityOS.Application.Common.Mapping;

public static class FacultyMapping
{
    public static FacultyResponse ToResponse(this Faculty faculty)
    {
        return new FacultyResponse
        {
            Id = faculty.Id,
            FirstName = faculty.FirstName,
            LastName = faculty.LastName,
            Email = faculty.Email,
            PhoneNumber = faculty.PhoneNumber,
            Title = faculty.Title,
            Department = faculty.Department,
            DistrictId = faculty.DistrictId,
            DistrictName = faculty.District?.Name,
            SchoolId = faculty.SchoolId,
            SchoolName = faculty.School?.Name,
            BeaconId = faculty.Beacon?.Id,
            BeaconDeviceName = faculty.Beacon?.DeviceName,
            BeaconSerialNumber = faculty.Beacon?.SerialNumber,
            BeaconType = faculty.Beacon?.Type,
            IsActive = faculty.IsActive,
            CreatedAt = faculty.CreatedAt,
            UpdatedAt = faculty.UpdatedAt
        };
    }

    public static IQueryable<FacultyResponse> ProjectToResponse(this IQueryable<Faculty> query)
    {
        return query.Select(f => new FacultyResponse
        {
            Id = f.Id,
            FirstName = f.FirstName,
            LastName = f.LastName,
            Email = f.Email,
            PhoneNumber = f.PhoneNumber,
            Title = f.Title,
            Department = f.Department,
            DistrictId = f.DistrictId,
            DistrictName = f.District != null ? f.District.Name : null,
            SchoolId = f.SchoolId,
            SchoolName = f.School != null ? f.School.Name : null,
            BeaconId = f.Beacon != null ? (int?)f.Beacon.Id : null,
            BeaconDeviceName = f.Beacon != null ? f.Beacon.DeviceName : null,
            BeaconSerialNumber = f.Beacon != null ? f.Beacon.SerialNumber : null,
            BeaconType = f.Beacon != null ? (BeaconType?)f.Beacon.Type : null,
            IsActive = f.IsActive,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });
    }

    public static Faculty ToEntity(this CreateFacultyRequest request)
    {
        var faculty = new Faculty(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.ToLower().Trim()
        );

        faculty.Update(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.ToLower().Trim(),
            request.PhoneNumber?.Trim(),
            request.Title?.Trim(),
            request.Department?.Trim()
        );

        if (request.DistrictId.HasValue)
            faculty.AssignToDistrict(request.DistrictId.Value);
        else if (request.SchoolId.HasValue)
            faculty.AssignToSchool(request.SchoolId.Value);

        return faculty;
    }

    public static void UpdateFromRequest(this Faculty faculty, UpdateFacultyRequest request)
    {
        faculty.Update(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.ToLower().Trim(),
            request.PhoneNumber?.Trim(),
            request.Title?.Trim(),
            request.Department?.Trim()
        );

        if (request.DistrictId.HasValue)
            faculty.AssignToDistrict(request.DistrictId.Value);
        else if (request.SchoolId.HasValue)
            faculty.AssignToSchool(request.SchoolId.Value);
    }
}
