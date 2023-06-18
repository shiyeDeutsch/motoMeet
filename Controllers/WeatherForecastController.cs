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



}
