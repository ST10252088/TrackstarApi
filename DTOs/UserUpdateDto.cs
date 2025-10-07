using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class UserUpdateDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? Surname { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? SignInMethod { get; set; }

        [StringLength(100)]
        public string? Uid { get; set; }

        public UserPreferencesDto? Preferences { get; set; }
    }
}
