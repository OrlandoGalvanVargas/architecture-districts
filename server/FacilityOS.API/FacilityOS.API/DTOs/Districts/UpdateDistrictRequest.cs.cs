using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Districts
{
    public class UpdateDistrictRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{5}$")]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}
