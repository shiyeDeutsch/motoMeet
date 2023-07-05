using Microsoft.AspNetCore.Mvc;
using motoMeet;
using Route = motoMeet.Route;

[ApiController]
[Route("api/")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    // GET: api/Routes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Route>>> GetRoutes()
    {
        return Ok(await _routeService.GetRoutes());
    }

    // GET: api/Routes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Route>> GetRoute(int id)
    {
        var route = await _routeService.GetRoute(id);

        if (route == null)
        {
            return NotFound();
        }

        return Ok(route);
    }

    // POST: api/Routes
    [HttpPost]
    public async Task<ActionResult<Route>> CreateRoute([FromBody] Route route)
    {
        await _routeService.CreateRoute(route);

        return CreatedAtAction(nameof(GetRoute), new { id = route.ID }, route);
    }

    // POST: api/Routes/5/Points
    [HttpPost("{id}/Points")]
    public async Task<ActionResult<RoutePoint>> AddPointToRoute(int id, [FromBody] RoutePoint point)
    {
        var route = await _routeService.GetRoute(id);

        if (route == null)
        {
            return NotFound();
        }

        await _routeService.AddPointToRoute(id, point);

        return CreatedAtAction(nameof(GetRoute), new { id = id }, route);
    }
}
