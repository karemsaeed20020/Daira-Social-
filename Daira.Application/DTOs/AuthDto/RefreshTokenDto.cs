using System.ComponentModel.DataAnnotations;

namespace Daira.Application.DTOs.AuthDto
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
        [Required(ErrorMessage = "RefreshToken is requird")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
