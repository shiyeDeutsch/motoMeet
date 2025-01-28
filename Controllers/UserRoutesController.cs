using motoMeet.Manager;

namespace motoMeet
{
    [ApiController]
[Route("api/[controller]")]
public class UserRoutesController : ControllerBase
{
    private readonly IUserRouteManager _userRouteManager;

    public UserRoutesController(IUserRouteManager userRouteManager)
    {
        _userRouteManager = userRouteManager;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserRoute>> GetUserRoute(int id)
    {
        var userRoute = await _userRouteManager.GetUserRouteAsync(id);
        if (userRoute == null)
            return NotFound();

        return Ok(userRoute);
    }

    // A user logs their usage of an existing route
    [HttpPost]
    public async Task<ActionResult<UserRoute>> CreateUserRoute([FromBody] CreateUserRouteRequest request)
    {
        var userRoute = await _userRouteManager.CreateUserRouteAsync(request);
        if (userRoute == null)
            return BadRequest("Could not create user route.");
        
        return Ok(userRoute);
    }
}

}