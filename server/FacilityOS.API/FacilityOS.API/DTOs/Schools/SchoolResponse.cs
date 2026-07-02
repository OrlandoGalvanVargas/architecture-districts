namespace FacilityOS.API.DTOs.Schools
{
    public class SchoolResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SchoolCode { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ContactEmail { get; set; }
        public int StudentCapacity { get; set; }
        public bool IsActive { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
