using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required] public string UserName { get; set; } = "";
    public required string Gender { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public DateOnly DOB { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = "";
    [Required] [MinLength(8)] public string Password { get; set; } = "";
}