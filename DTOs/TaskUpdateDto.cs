using System;
using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class TaskUpdateDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public string? AssignedTo { get; set; }
        public string? ColorStatus { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
