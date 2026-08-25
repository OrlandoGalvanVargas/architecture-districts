using FacilityOS.API.Models.Base;

namespace FacilityOS.API.Models;

public class District : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ICollection<School> Schools { get; private set; } = new List<School>();
    public ICollection<Beacon> Beacons { get; private set; } = new List<Beacon>();  
    public ICollection<Faculty> Faculties { get; private set; } = new List<Faculty>();

    private District() { }

    public District(string name, string code, string state, string city, string zipCode, string address, string? description = null)
    {
        Name = name;
        Code = code;
        State = state;
        City = city;
        ZipCode = zipCode;
        Address = address;
        Description = description;
    }

    public void Update(string name, string code, string state, string city, string zipCode, string address, string? description = null)
    {
        Name = name;
        Code = code;
        State = state;
        City = city;
        ZipCode = zipCode;
        Address = address;
        Description = description;
    }
}