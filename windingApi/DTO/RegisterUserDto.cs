using System.ComponentModel.DataAnnotations;
using System.Text;

namespace windingApi.DTO;

public class RegisterUserDto
{
    [Required]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "password must be minimum {2} and maximum {1} long!")]
    public string Password { get; set; }
}