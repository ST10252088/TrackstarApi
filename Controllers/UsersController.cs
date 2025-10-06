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

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _firestore.GetAllUsersAsync();
            return Ok(users);
        }

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
