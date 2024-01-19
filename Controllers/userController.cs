using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace motoMeet
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UsersController(UserManager userManager)
        {
            _userManager = userManager;
        }
         [Authorize]
         [HttpGet]
        public async Task<IActionResult> getAll()
        {
            var users = await _userManager.GetUsers();
            return Ok(users);
        }



        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var user = await _userManager.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/users
         
        [HttpPost]
        public async Task<IActionResult> create([FromBody] Person user)
        {
            var (newUser, token) = await _userManager.CreateUser(user);
            return Ok(new { User = newUser, Token = token });
        }


        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, [FromBody] Person updatedUser)
        {
            var user = await _userManager.UpdateUser(id, updatedUser);

            if (user == null)
                return NotFound();
            return Ok(user);
        }

        //     // DELETE: api/users/5
        //     [HttpDelete("{id}")]
        //     public async Task<IActionResult> Delete(int id)
        //     {
        //         bool success = await _repository.DeleteUser(id);
        //         if (success)
        //         {
        //             return Ok();
        //         }

        //         //   await _repository.SaveAsync();
        //         return NotFound();
        //     }
        // }


    }
}