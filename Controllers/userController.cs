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

        //GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.GetUsers();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Create([FromBody] Person user)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newUser = await _userManager.CreateUser(user);
            //  await _repository.SaveAsync();
            //  return CreatedAtAction(nameof(GetById), new { id = user.ID }, user);
            return Ok(newUser);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Person updatedUser)
        {
            var user =await  _userManager.UpdateUser(id, updatedUser);

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