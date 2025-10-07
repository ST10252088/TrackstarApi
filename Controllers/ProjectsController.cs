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

        // Retrieves all projects from database
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _firestore.GetProjectsAsync();
            return Ok(projects);
        }

        // Creates a new project
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _firestore.CreateProjectAsync(dto.Name, dto.Description);
            return CreatedAtAction(nameof(GetProjectById), new { id }, new { id });
        }

        // Retrieves a specific project by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(string id)
        {
            var project = await _firestore.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            project["id"] = id;
            return Ok(project);
        }

        // Edits a specific project by Id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] ProjectUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid request body." });

            var updates = new Dictionary<string, object>();
            if (dto.Name != null) updates["name"] = dto.Name;
            if (dto.Description != null) updates["description"] = dto.Description;
            if (dto.MemberUids != null) updates["memberUids"] = dto.MemberUids;

            if (updates.Count == 0)
                return BadRequest(new { message = "No fields provided for update." });

            var ok = await _firestore.UpdateProjectAsync(id, updates);
            if (!ok)
                return NotFound(new { message = "Project not found." });

            return Ok(new { message = "Project updated successfully." });
        }


        // Deletes a specific Project by Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var ok = await _firestore.DeleteProjectAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Assigns a user to a project accepting the project Id and the user uid
        [HttpPost("{id}/assign-user")]
        public async Task<IActionResult> AssignUser(string id, [FromBody] AssignUserDto dto)
        {
            var ok = await _firestore.AssignUserToProjectAsync(id, dto.UserUid);
            if (!ok) return NotFound();
            return Ok(new { message = "User assigned successfully" });
        }
    }
}
