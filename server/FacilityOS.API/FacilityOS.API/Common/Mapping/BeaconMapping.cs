using FacilityOS.API.DTOs.Beacons;
using FacilityOS.API.Models;

namespace FacilityOS.API.Common.Mapping;

public static class BeaconMapping
{
    public static BeaconResponse ToResponse(this Beacon beacon)
    {
        return new BeaconResponse
        {
            Id = beacon.Id,
            DeviceName = beacon.DeviceName,
            SerialNumber = beacon.SerialNumber,
            Type = beacon.Type,
            Status = beacon.Status,
            DistrictId = beacon.DistrictId,
            DistrictName = beacon.District?.Name,
            SchoolId = beacon.SchoolId,
            SchoolName = beacon.School?.Name,
            FacultyId = beacon.FacultyId,
            FacultyName = beacon.Faculty != null
                ? $"{beacon.Faculty.FirstName} {beacon.Faculty.LastName}".Trim()
                : null,
            CreatedAt = beacon.CreatedAt,
            UpdatedAt = beacon.UpdatedAt
        };
    }

    public static IQueryable<BeaconResponse> ProjectToResponse(this IQueryable<Beacon> query)
    {
        return query.Select(b => new BeaconResponse
        {
            Id = b.Id,
            DeviceName = b.DeviceName,
            SerialNumber = b.SerialNumber,
            Type = b.Type,
            Status = b.Status,
            DistrictId = b.DistrictId,
            DistrictName = b.District != null ? b.District.Name : null,
            SchoolId = b.SchoolId,
            SchoolName = b.School != null ? b.School.Name : null,
            FacultyId = b.FacultyId,
            FacultyName = b.Faculty != null
                ? b.Faculty.FirstName + " " + b.Faculty.LastName
                : null,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        });
    }

    public static Beacon ToEntity(this CreateBeaconRequest request)
    {
        var beacon = new Beacon(
            request.DeviceName.Trim(),
            request.SerialNumber.Trim().ToUpper(),
            request.Type
        );

        if (request.DistrictId.HasValue)
            beacon.AssignToDistrict(request.DistrictId.Value);
        else if (request.SchoolId.HasValue)
            beacon.AssignToSchool(request.SchoolId.Value);

        return beacon;
    }

    public static void UpdateFromRequest(this Beacon beacon, UpdateBeaconRequest request)
    {
        beacon.Update(
            request.DeviceName.Trim(),
            request.Type,
            request.Status
        );

        if (request.DistrictId.HasValue)
        {
            beacon.AssignToDistrict(request.DistrictId.Value);
        }
        else if (request.SchoolId.HasValue)
        {
            beacon.AssignToSchool(request.SchoolId.Value);
        }
        else
        {
            beacon.Unassign();
        }
    }
}
