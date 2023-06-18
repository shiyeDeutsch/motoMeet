using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
 
 
namespace motoMeet.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }



}
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly MotoMeetDbContext _dbContext;

    public UsersController(MotoMeetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _dbContext.Persons.ToList(); // Assuming your Persons table maps to the Person class

        return Ok(users);
    }
    [HttpPost(Name = "addUser")]
    public IActionResult CreateUser([FromBody] Person person)
    {
        if (person == null)
        {
            return BadRequest("Person object is null");
        }

        // Add new person to the Persons table
        _dbContext.Persons.Add(person);
        _dbContext.SaveChanges();

        return CreatedAtRoute("GetUserById", new { id = person.ID }, person);
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public IActionResult GetUser(long id)
    {
        var user = _dbContext.Persons.FirstOrDefault(u => u.ID == id);
        if (user == null)
        {
            return NotFound();
        }
        return new ObjectResult(user);
    }
    [HttpGet("allusersStartWithA")]
    public IActionResult GetAllUsersStartWithA()
    {
        var users = _dbContext.Persons.Where(u => u.FirstName.StartsWith("A")).ToList();

        return Ok(users);
    }



}
[ApiController]
[Route("[Controller]")]
public class EntitiesController : ControllerBase
{
    private readonly IRepository<Person> _repository;

    public EntitiesController(IRepository<Person> repository)
    {
        _repository = repository;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Person updatedEntity)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        // Copy the fields from updatedEntity onto entity...
        // This will depend on the structure of Entity

        _repository.Update(entity);
        await _repository.SaveAsync();

        return NoContent(); // Return a success status code
    }

[HttpPatch("{id}")]
public async Task<IActionResult> UpdatePartial(int id, [FromBody] JsonPatchDocument<Person> patchDocument)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
    {
        return NotFound();
    }

    // Apply the updates from the patch document to the entity
    patchDocument.ApplyTo(entity);

    // Check if the updated entity is valid
    if (!TryValidateModel(entity))
    {
        return BadRequest(ModelState);
    }

    // Update the entity and save changes
    _repository.Update(entity);
    await _repository.SaveAsync();

    return NoContent(); // Return a success status code
}


}
