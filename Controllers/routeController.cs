using Microsoft.AspNetCore.Mvc;
 
namespace motoMeet
{
    [ApiController]
[Route("api/routes")]
public class RoutesController : ControllerBase
{
    private readonly RouteManager _routeManager;

    public RoutesController(RouteManager routeManager)
    {
        _routeManager = routeManager;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Route>> GetRoute(int id)
    {
        var route = await _routeManager.GetRoute(id);
        if (route == null)
            return NotFound();

        return Ok(route);
    }

    // Create the official route definition
    [HttpPost]
    public async Task<ActionResult<Route>> CreateRoute([FromBody] CreateRouteRequest request)
    {
        // This will NOT update the user’s distance,
        // because this is just the official route definition.
        var createdRoute = await _routeManager.CreateRouteOfficialAsync(request);

        return CreatedAtAction(nameof(GetRoute), new { id = createdRoute.Id }, createdRoute);
    }
}

}