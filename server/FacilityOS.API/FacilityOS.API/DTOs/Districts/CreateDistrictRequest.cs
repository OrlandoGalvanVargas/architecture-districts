using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Districts
{
    public class CreateDistrictRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must be uppercase letters and numbers only")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "ZipCode is required")]
        [MaxLength(100)]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "ZipCode must be 5 digits")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }    
    }
}
