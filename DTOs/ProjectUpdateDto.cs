using System.ComponentModel.DataAnnotations;

namespace Trackstar.Api.DTOs
{
    public class ProjectUpdateDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public List<string>? MemberUids { get; set; }
    }
}
