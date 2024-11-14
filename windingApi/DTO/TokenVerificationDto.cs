using System.ComponentModel.DataAnnotations;

namespace windingApi.DTO;

public class TokenVerificationDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Email { get; set; }
}