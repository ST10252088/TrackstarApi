using Microsoft.AspNetCore.Mvc;
using Trackstar.Api.DTOs;
using Trackstar.Api.Services;

namespace Trackstar.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly FirestoreService _firestore;
        public TasksController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(string projectId)
        {
            var tasks = await _firestore.GetTasksAsync(projectId);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(string projectId, [FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _firestore.CreateTaskAsync(projectId, dto.Title, dto.Description);
            return CreatedAtAction(nameof(GetTasks), new { projectId }, new { id });
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(string projectId, string taskId, [FromBody] TaskUpdateDto dto)
        {
            var updates = new Dictionary<string, object>();
            if (dto.Title != null) updates["title"] = dto.Title;
            if (dto.Description != null) updates["description"] = dto.Description;

            if (updates.Count == 0)
                return BadRequest(new { message = "No fields to update" });

            var ok = await _firestore.UpdateTaskAsync(projectId, taskId, updates);
            if (!ok) return NotFound();
            return Ok(new { updated = true });
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(string projectId, string taskId)
        {
            var ok = await _firestore.DeleteTaskAsync(projectId, taskId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
