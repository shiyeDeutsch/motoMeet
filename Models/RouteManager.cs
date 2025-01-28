using NetTopologySuite.Geometries;

namespace motoMeet
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
       
            public async Task<Route> GetRoute(int id)
    {
        var route = await _routeService.GetRoute(id);
        if (route == null)
            throw new Exception($"No route found with ID {id}.");
        return route;
    }


      public async Task<Route> CreateRouteOfficialAsync(CreateRouteRequest request)
    {
        // 1. Fetch tags
        var routeTags = await _routeService.GetTags(
            new GetByIdsSpecification<Tag>(request.RouteTagsIds)
        );

        // 2. Convert the geometry from request
        var startPoint = ToNetTopologyPoint(request.StartPoint);
        var endPoint   = ToNetTopologyPoint(request.EndPoint);
        var routePoints = request.RoutePoints
            .Select((p, i) => new RoutePoint
            {
                SequenceNumber = i,
                Point = ToNetTopologyPoint(p)
            })
            .ToList();

        // 3. Build the Route entity
        var route = new Route
        {
            PersonId = request.CreatorId,  // The route’s creator
            Name = request.Name,
            Description = request.Description,
            StartPoint = startPoint,
            EndPoint = endPoint,
            Length = request.Length,
            Duration = request.Duration,

            DifficultyLevelId = request.DifficultyLevelId,
            RouteTypeId = request.RouteTypeId,

            RoutePoints = routePoints,
            Tags = routeTags
        };

        // 4. Create official route in DB
        var createdRoute = await _routeService.CreateRoute(route);

        // 5. Do geocoding if you want to store country/region
        try
        {
            (string country, string region) = await _geocodingService.GetCountryAndRegion(
                startPoint.Y, // latitude
                startPoint.X  // longitude
            );
            createdRoute.Country = country;
            createdRoute.Region = region;
            await _routeService.UpdateRoute(createdRoute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geocoding failed for route {RouteId}", createdRoute.Id);
        }

        // IMPORTANT: We DO NOT update user’s total distance here, because
        // this is the official route creation, not a user traveling the route.

        return createdRoute;
    }
          private Point ToNetTopologyPoint(GeoPoint gp)
    {
        return new Point(new CoordinateZ(gp.latitude, gp.longitude, gp.altitude ?? 0.0))
        {
            SRID = 4326
        };
    }
    }
    public interface IRouteManager
{
    Task<Route> GetRoute(int id);
    Task<Route> CreateRouteOfficialAsync(CreateRouteRequest request);
}
}
