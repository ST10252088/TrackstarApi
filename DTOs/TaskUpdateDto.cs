using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class TaskUpdateDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}
