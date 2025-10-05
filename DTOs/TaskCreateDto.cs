using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}
