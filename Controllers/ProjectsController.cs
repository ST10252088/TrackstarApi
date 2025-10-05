using Microsoft.AspNetCore.Mvc;
using Trackstar.Api.DTOs;
using Trackstar.Api.Services;

namespace Trackstar.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly FirestoreService _firestore;
        public ProjectsController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _firestore.GetProjectsAsync();
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _firestore.CreateProjectAsync(dto.Name, dto.Description);
            return CreatedAtAction(nameof(GetProjectById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(string id)
        {
            var project = await _firestore.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            project["id"] = id;
            return Ok(project);
        }
    }
}
