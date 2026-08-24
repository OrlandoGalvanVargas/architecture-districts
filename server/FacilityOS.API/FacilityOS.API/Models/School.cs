using FacilityOS.API.Models.Base;
using FacilityOS.API.Models.Enums;

namespace FacilityOS.API.Models;

public class School : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string SchoolCode { get; private set; } = string.Empty;
    public SchoolLevel Level { get; private set; }
    public SchoolType Type { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? ContactEmail { get; private set; }
    public int StudentCapacity { get; private set; }
    public int DistrictId { get; private set; }
    public District District { get; private set; } = null!;

    private School() { } 

    public School(string name, string schoolCode, SchoolLevel level, SchoolType type,
        string address, string city, string state, string zipCode, int districtId,
        int studentCapacity = 0, string? phone = null, string? contactEmail = null)
    {
        Name = name;
        SchoolCode = schoolCode;
        Level = level;
        Type = type;
        Address = address;
        City = city;
        State = state;
        ZipCode = zipCode;
        DistrictId = districtId;
        StudentCapacity = studentCapacity;
        Phone = phone;
        ContactEmail = contactEmail;
    }

    public void Update(string name, string schoolCode, SchoolLevel level, SchoolType type,
        string address, string city, string state, string zipCode, int studentCapacity,
        string? phone = null, string? contactEmail = null)
    {
        Name = name;
        SchoolCode = schoolCode;
        Level = level;
        Type = type;
        Address = address;
        City = city;
        State = state;
        ZipCode = zipCode;
        StudentCapacity = studentCapacity;
        Phone = phone;
        ContactEmail = contactEmail;
    }
}
