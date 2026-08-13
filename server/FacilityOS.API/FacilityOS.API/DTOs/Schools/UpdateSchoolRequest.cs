using FacilityOS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Schools
{
    public class UpdateSchoolRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string SchoolCode { get; set; } = string.Empty;
        [Required]
        public SchoolLevel Level { get; set; }
        [Required]
        public SchoolType Type { get; set; }
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\d{5}$")]
        public string ZipCode { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Phone { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string? ContactEmail { get; set; }
        [Range(1, 10000)]
        public int StudentCapacity { get; set; }
        [Required(ErrorMessage = "DistrictId is required")]
        public int DistrictId { get; set; }
        public bool IsActive { get; set; }
    }
}
