namespace motoMeet.Manager
{

    public class RouteManager
    {
        private readonly IRouteService _routeService;
        public RouteManager(IRouteService routeService)
        {
            _routeService = routeService;
        }
        public async Task<IEnumerable<Route>> GetRoutes()
        {
            try
            {
                var routes = await _routeService.GetRoutes();
                return routes;
            }
            catch (Exception ex)
            {
                // Log the exception, re-throw it, or handle it in a way that makes sense for your application
                throw new Exception("An error occurred while fetching the routes.", ex);
            }
        }
        public async Task<Route> GetRoute(int id)
        {
            try
            {
                var route = await _routeService.GetRoute(id);
                if (route == null)
                {
                    throw new Exception($"No route found with ID {id}.");
                }
                return route;
            }
            catch (Exception ex)
            {
                // Log the exception, re-throw it, or handle it in a way that makes sense for your application
                throw new Exception($"An error occurred while fetching the route with ID {id}.", ex);
            }
        }

        public async Task<Route> CreateRoute(NewRouteModel newRoute)
        {
 ///  gpt please imploment here!!
        }
    }

}