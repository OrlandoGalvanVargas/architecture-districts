using System.ComponentModel.DataAnnotations;

namespace FacilityOS.API.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
