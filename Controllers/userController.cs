using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
 
 
namespace motoMeet.Controllers;
// [Route("api/users")]
// [ApiController]
// public class UsersController : ControllerBase
// {
//     private readonly MotoMeetDbContext _dbContext;

//     public UsersController(MotoMeetDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }

//     [HttpGet]
//     public IActionResult GetAllUsers()
//     {
//         var users = _dbContext.Persons.ToList(); // Assuming your Persons table maps to the Person class

//         return Ok(users);
//     }
//     [HttpPost(Name = "addUser")]
//     public IActionResult CreateUser([FromBody] Person person)
//     {
//         if (person == null)
//         {
//             return BadRequest("Person object is null");
//         }

//         // Add new person to the Persons table
//         _dbContext.Persons.Add(person);
//         _dbContext.SaveChanges();

//         return CreatedAtRoute("GetUserById", new { id = person.ID }, person);
//     }

//     [HttpGet("{id}", Name = "GetUserById")]
//     public IActionResult GetUser(long id)
//     {
//         var user = _dbContext.Persons.FirstOrDefault(u => u.ID == id);
//         if (user == null)
//         {
//             return NotFound();
//         }
//         return new ObjectResult(user);
//     }
//     [HttpGet("allusersStartWithA")]
//     public IActionResult GetAllUsersStartWithA()
//     {
//         var users = _dbContext.Persons.Where(u => u.FirstName.StartsWith("A")).ToList();

//         return Ok(users);
//     }



// }
// [ApiController]
// [Route("[Controller]")]
// public class EntitiesController : ControllerBase
// {
//     private readonly IRepository<Person> _repository;

//     public EntitiesController(IRepository<Person> repository)
//     {
//         _repository = repository;
//     }

//     [HttpPut("{id}")]
//     public async Task<IActionResult> Update(int id, [FromBody] Person updatedEntity)
//     {
//         var entity = await _repository.GetByIdAsync(id);
//         if (entity == null)
//         {
//             return NotFound();
//         }

//         // Copy the fields from updatedEntity onto entity...
//         // This will depend on the structure of Entity

//         _repository.Update(entity);
//         await _repository.SaveAsync();

//         return NoContent(); // Return a success status code
//     }

// [HttpPatch("{id}")]
// public async Task<IActionResult> UpdatePartial(int id, [FromBody] JsonPatchDocument<Person> patchDocument)
// {
//     var entity = await _repository.GetByIdAsync(id);
//     if (entity == null)
//     {
//         return NotFound();
//     }

//     // Apply the updates from the patch document to the entity
//     patchDocument.ApplyTo(entity);

//     // Check if the updated entity is valid
//     if (!TryValidateModel(entity))
//     {
//         return BadRequest(ModelState);
//     }

//     // Update the entity and save changes
//     _repository.Update(entity);
//     await _repository.SaveAsync();

//     return NoContent(); // Return a success status code
// }
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRepository<Person> _repository;

    public UsersController(IRepository<Person> repository)
    {
        _repository = repository;
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
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
        await _repository.AddAsync(user);
        await _repository.SaveAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.ID }, user);
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Person updatedUser)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.RoleID = updatedUser.RoleID;
        user.NationalCode = updatedUser.NationalCode;
        user.AddedOn = updatedUser.AddedOn;
        user.EditOn = DateTime.Now;

        _repository.Update(user);
        await _repository.SaveAsync();

        return NoContent();
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        _repository.Delete(user);
        await _repository.SaveAsync();
        return NoContent();
    }
}



