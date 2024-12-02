using System.ComponentModel.DataAnnotations;

namespace windingApi.DTO;

public class EmailConfirmDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Token { get; set; }
}