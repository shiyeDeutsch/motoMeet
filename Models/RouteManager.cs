namespace motoMeet.Manager
{

    public class RouteManager
    {
        private readonly IRouteService _routeService;
        private readonly IGeocodingService _geocodingService;
        private readonly IUserService _userService;
        private readonly ILogger<RouteManager> _logger;
        public RouteManager(IRouteService routeService, IGeocodingService geocodingService,
        IUserService userService,
        ILogger<RouteManager> logger)
        {
            _routeService = routeService;
            _geocodingService = geocodingService;
            _userService = userService;
            _logger = logger;
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
        {
            var routeTags = await _routeService.GetTags(new GetByIdsSpecification<Tag>(newRouteModel.RouteTagsIds));
            // Convert NewRouteModel to Route entity
            var route = new Route
            {
                Tags = routeTags,
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

            // Fetch geographic details
            try
            {
                var startPoint = createdRoute.StartPoint;
                var (country, region) = await _geocodingService.GetCountryAndRegion(
                    startPoint.Y, // Latitude
                    startPoint.X  // Longitude
                );
                createdRoute.Country = country;
                createdRoute.Region = region;
                await _routeService.UpdateRoute(createdRoute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Geocoding failed for route {RouteId}", createdRoute.Id);
            }

            // Update user's total distance
            try
            {
                var user = await _userService.GetUser(createdRoute.AddedBy);
                user.TotalDistance += createdRoute.Length;
                await _userService.UpdateUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update distance for user {UserId}", createdRoute.AddedBy);
            }


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