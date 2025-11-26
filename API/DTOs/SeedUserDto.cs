namespace API.DTOs;

public class SeedUserDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public DateOnly DOB { get; set; }
    public string? ImageUrl { get; set; }
    public required string UserName { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}
