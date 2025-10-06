using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class AssignUserDto
    {
        [Required]
        public string UserUid { get; set; } = null!;
    }
}
