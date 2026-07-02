using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.Models
{
    public class School
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string SchoolCode { get; set} = string.Empty;
        [Required]
        public SchoolLevel Level { get; set; }
        [Required]
        public SchoolType Type { get; set; }
        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;
        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }
        public int StudenCapacity { get; set; }
        public bool isActive { get; set; } = true;
        public int DistrictId { get; set; }
        public District District { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum SchoolLevel
    {
        Elementary,
        Middle,
        High,
        K12,
        Prek
    }

    public enum SchoolType
    {
        Public,
        Charter,
        Magnet,
        Alternative
    }
}
