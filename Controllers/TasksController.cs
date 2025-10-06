using Microsoft.AspNetCore.Mvc;
using Trackstar.Api.DTOs;
using Trackstar.Api.Services;
using Google.Cloud.Firestore;

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

        // Gets all tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks(string projectId)
        {
            var tasks = await _firestore.GetTasksAsync(projectId);
            return Ok(tasks);
        }

        // Get task by id
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(string projectId, string taskId)
        {
            var task = await _firestore.GetTaskByIdAsync(projectId, taskId);
            if (task == null) return NotFound();
            task["id"] = taskId;
            return Ok(task);
        }

        // Creates a task
        [HttpPost]
        public async Task<IActionResult> CreateTask(string projectId, [FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var data = new Dictionary<string, object>
            {
                ["name"] = dto.Name,
                ["description"] = dto.Description ?? "",
                ["assignedTo"] = dto.AssignedTo,
                ["colorStatus"] = dto.ColorStatus,
                ["status"] = dto.Status,
                ["dueDate"] = Timestamp.FromDateTime(dto.DueDate.ToUniversalTime())
            };

            var id = await _firestore.CreateTaskAsync(projectId, data);
            return CreatedAtAction(nameof(GetTaskById), new { projectId, taskId = id }, new { id });
        }

        // Edits or updates a task by id
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(string projectId, string taskId, [FromBody] TaskUpdateDto dto)
        {
            var updates = new Dictionary<string, object>();
            if (dto.Name != null) updates["name"] = dto.Name;
            if (dto.Description != null) updates["description"] = dto.Description;
            if (dto.AssignedTo != null) updates["assignedTo"] = dto.AssignedTo;
            if (dto.ColorStatus != null) updates["colorStatus"] = dto.ColorStatus;
            if (dto.Status != null) updates["status"] = dto.Status;
            if (dto.DueDate.HasValue)
                updates["dueDate"] = Timestamp.FromDateTime(dto.DueDate.Value.ToUniversalTime());

            if (updates.Count == 0)
                return BadRequest(new { message = "No fields to update" });

            var ok = await _firestore.UpdateTaskAsync(projectId, taskId, updates);
            if (!ok) return NotFound();
            return Ok(new { updated = true });
        }

        // deletes a task by id
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(string projectId, string taskId)
        {
            var ok = await _firestore.DeleteTaskAsync(projectId, taskId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
