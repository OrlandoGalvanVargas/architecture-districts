using FacilityOS.API.Models.Base;
using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.Models;

public class Beacon : AuditableEntity
{
    public string DeviceName { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public BeaconType Type { get; private set; }
    public BeaconStatus Status { get; private set; } = BeaconStatus.Available;
    public int? DistrictId { get; private set; }
    public District? District { get; private set; }
    public int? SchoolId { get; private set; }
    public School? School { get; private set; }
    public int? FacultyId { get; private set; }
    public Faculty? Faculty { get; private set; }

    private Beacon() { } 

    public Beacon(string deviceName, string serialNumber, BeaconType type)
    {
        DeviceName = deviceName;
        SerialNumber = serialNumber;
        Type = type;
    }

    public void Update(string deviceName, BeaconType type, BeaconStatus status)
    {
        DeviceName = deviceName;
        Type = type;
        Status = status;
    }

    public void AssignToDistrict(int districtId)
    {
        ClearAssignment();
        DistrictId = districtId;
        Status = BeaconStatus.Assigned;
    }

    public void AssignToSchool(int schoolId)
    {
        ClearAssignment();
        SchoolId = schoolId;
        Status = BeaconStatus.Assigned;
    }

    public void AssignToFaculty(int facultyId)
    {
        ClearAssignment();
        FacultyId = facultyId;
        Status = BeaconStatus.Assigned;
    }

    public void Unassign()
    {
        ClearAssignment();
        Status = BeaconStatus.Available;
    }

    private void ClearAssignment()
    {
        DistrictId = null;
        SchoolId = null;
        FacultyId = null;
    }
}