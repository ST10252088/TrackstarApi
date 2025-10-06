using System;
using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public string AssignedTo { get; set; } = null!; // user email

        [Required]
        public string ColorStatus { get; set; } = "#00B7FF"; // default blue

        [Required]
        public string Status { get; set; } = "To Do"; // default status

        [Required]
        public DateTime DueDate { get; set; }
    }
}
