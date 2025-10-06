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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0)
                return BadRequest(new { message = "No updates provided" });

            var ok = await _firestore.UpdateProjectAsync(id, updates);
            if (!ok) return NotFound();
            return Ok(new { updated = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var ok = await _firestore.DeleteProjectAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/assign-user")]
        public async Task<IActionResult> AssignUser(string id, [FromBody] AssignUserDto dto)
        {
            var ok = await _firestore.AssignUserToProjectAsync(id, dto.UserUid);
            if (!ok) return NotFound();
            return Ok(new { message = "User assigned successfully" });
        }
    }
}
