using System.ComponentModel.DataAnnotations;
using Trackstar.Api.DTOs;

public class UserCreateDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string Surname { get; set; } = null!;

    public string? Phone { get; set; }
    public string? SignInMethod { get; set; }
    public string? Uid { get; set; }

    public UserPreferencesDto? Preferences { get; set; }
}
