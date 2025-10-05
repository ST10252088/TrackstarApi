using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class ProjectCreateDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
