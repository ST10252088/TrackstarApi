using Microsoft.AspNetCore.Mvc;
using Trackstar.Api.Services;

namespace Trackstar.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly FirestoreService _firestore;
        public UsersController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        // Updating User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0)
                return BadRequest(new { message = "No updates provided." });

            var ok = await _firestore.UpdateUserAsync(id, updates);
            if (!ok)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User updated successfully." });
        }

        // Reading all users from database
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _firestore.GetAllUsersAsync();
            return Ok(users);
        }

        // reading one user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _firestore.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            user["id"] = id;
            return Ok(user);
        }
    }
}
