using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
