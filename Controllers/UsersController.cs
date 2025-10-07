using Microsoft.AspNetCore.Mvc;
using Trackstar.Api.Services;
using Trackstar.Api.DTOs;

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
        // --- CREATE ---
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = new Dictionary<string, object>
            {
                ["email"] = dto.Email,
                ["firstName"] = dto.FirstName,
                ["surname"] = dto.Surname,
                ["phone"] = dto.Phone ?? "",
                ["signInMethod"] = dto.SignInMethod ?? "",
                ["uid"] = dto.Uid ?? ""
            };

            var id = await _firestore.CreateUserAsync(data);
            return CreatedAtAction(nameof(GetUserById), new { id }, new { id });
        }

        // --- UPDATE ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request body." });

            var updates = new Dictionary<string, object>();

            if (dto.FirstName != null) updates["firstName"] = dto.FirstName;
            if (dto.Surname != null) updates["surname"] = dto.Surname;
            if (dto.Email != null) updates["email"] = dto.Email;
            if (dto.Phone != null) updates["phone"] = dto.Phone;
            if (dto.SignInMethod != null) updates["signInMethod"] = dto.SignInMethod;
            if (dto.Uid != null) updates["uid"] = dto.Uid;

            if (updates.Count == 0)
                return BadRequest(new { message = "No fields provided for update." });

            var ok = await _firestore.UpdateUserAsync(id, updates);
            if (!ok)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User updated successfully." });
        }

        // READ ALL USERS
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

        // --- DELETE ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var ok = await _firestore.DeleteUserAsync(id);
            if (!ok)
                return NotFound(new { message = "User not found." });

            return NoContent();
        }
    }
}
