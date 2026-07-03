using FacilityOS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Schools
{
    public class CreateSchoolRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "SchoolCode is required")]
        [MaxLength(50)]
        public string SchoolCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Level is required")]
        public SchoolLevel Level { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public SchoolType Type { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "City is required")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "State is required")]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;
        [Required(ErrorMessage = "ZipCode is required")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "ZipCode must be 5 digits")]
        public string ZipCode { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Phone { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string? ContactEmail { get; set; }
        [Range(1, 10000, ErrorMessage = "StudentCapacity must be between 1 and 10000")]
        public int StudentCapacity { get; set; }
        [Required(ErrorMessage = "DistrictId is required")]
        public int DistrictId { get; set; }
    }
}
