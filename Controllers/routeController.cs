using Microsoft.AspNetCore.Mvc;
using motoMeet.Manager;

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

        // GET: api/Routes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Route>>> GetRoutes()
        {
            var routes = await _routeManager.GetRoutes();
            if (routes == null)
                return NotFound();
            return Ok(routes);

        }

        // GET: api/Routes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Route>> GetRoute(int id)
        {
            var route = await _routeManager.GetRoute(id);

            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        // POST: api/Routes
        [HttpPost]
        public async Task<ActionResult<Route>> CreateRoute([FromBody] NewRouteModel route)
        {
            await _routeManager.CreateRoute(route);

        //    return CreatedAtAction(nameof(GetRoute), new { id = route.ID }, route);
        }

        // POST: api/Routes/5/Points
        // [HttpPost("{id}/Points")]
        // public async Task<ActionResult<RoutePoint>> AddPointToRoute(int id, [FromBody] RoutePoint point)
        // {
        //     var route = await _routeService.GetRoute(id);

        //     if (route == null)
        //     {
        //         return NotFound();
        //     }

        //     await _routeService.AddPointToRoute(id, point);

        //     return CreatedAtAction(nameof(GetRoute), new { id = id }, route);
        // }
    }
}