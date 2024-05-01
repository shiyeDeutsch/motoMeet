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

        public async Task<Route?> CreateRoute(NewRouteModel newRouteModel)
        { var routeTags=await  _routeService.GetTags(new GetByIdsSpecification<Tag>(newRouteModel.RouteTagsIds));
            // Convert NewRouteModel to Route entity
            var route = new Route
            {Tags=routeTags,
                AddedBy = newRouteModel.AddedBy,
                Name = newRouteModel.Name,
                Description = newRouteModel.Description,
                StartPoint = newRouteModel.StartPoint,
                EndPoint = newRouteModel.EndPoint,
                Length = newRouteModel.Length,
                Duration = newRouteModel.Duration,
                RoutePoints = newRouteModel.RoutePoints.Select((p, index) => new RoutePoint
                {
                    SequenceNumber = index,
                    Point = p,
                }).ToList()
            };
         
            // Add the Route entity to the DbContext and save to generate its ID
            var createdRoute = await _routeService.CreateRoute(route);
            return createdRoute;
        }
        //     if (createdRoute != null)

        //     {
        //         var routePoints = newRouteModel.RoutePoints.Select((point, index) => new RoutePoint
        //         {
        //             SequenceNumber = index,
        //             RouteId = route.Id, // Use generated ID
        //             Point = point,
        //             // Set other RoutePoint properties as necessary...
        //         }).ToList();

        //         // Use the new AddRangeAsync method to batch insert points
        //    //     await _routeService.AddPointsToRoute(route.Id, routePoints);
        //     //    await _routeService.AddTagsToRoute(route.Id, newRouteModel.RouteTagsIds);

        //         return route;
        //     }
        //     return null;
        // }

    }
}