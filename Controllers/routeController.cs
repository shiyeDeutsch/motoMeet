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
        public async Task<ActionResult<Route>> CreateRoute([FromBody] NewRouteModel routesdfgas)
        {
            // Console.Write("request get in");
            // try
            // {
                var createdRoute = await _routeManager.CreateRoute(routesdfgas);

                // Assuming 'GetRoute' is an existing method that can fetch a route by its ID
                // and that 'createdRoute.Id' holds the ID of the newly created route.
                return CreatedAtAction(nameof(GetRoute), new { id = createdRoute.Id }, createdRoute);
            // }
            // catch (Exception ex)
            // {
            //     // Consider logging the exception details here
            //     return StatusCode(500, "An error occurred while creating the route.");
            // }
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