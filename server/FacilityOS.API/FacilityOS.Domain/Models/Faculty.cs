using FacilityOS.Domain.Models.Base;

namespace FacilityOS.Domain.Models;

public class Faculty : AuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Title { get; private set; }
    public string? Department { get; private set; }
    public int? DistrictId { get; private set; }
    public District? District { get; private set; }
    public int? SchoolId { get; private set; }
    public School? School { get; private set; }
    public Beacon? Beacon { get; private set; }

    private Faculty() { }

    public Faculty(string firstName, string lastName, string email, string? phoneNumber = null,
        string? title = null, string? department = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Title = title;
        Department = department;
    }

    public void Update(string firstName, string lastName, string email, string? phoneNumber = null,
        string? title = null, string? department = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Title = title;
        Department = department;
    }

    public void AssignToDistrict(int districtId)
    {
        ClearEntityAssignment();
        DistrictId = districtId;
    }

    public void AssignToSchool(int schoolId)
    {
        ClearEntityAssignment();
        SchoolId = schoolId;
    }

    private void ClearEntityAssignment()
    {
        DistrictId = null;
        SchoolId = null;
    }
}