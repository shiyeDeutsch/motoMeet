 
using NetTopologySuite.Geometries;

namespace motoMeet
{
    public interface IUserRouteManager
{
    Task<UserRoute?> GetUserRouteAsync(int id);
    Task<UserRoute?> CreateUserRouteAsync(CreateUserRouteRequest request);
}

public class UserRouteManager : IUserRouteManager
{
    private readonly IRouteService _routeService;
    private readonly IUserRouteService _userRouteService;
    private readonly IUserService _userService;
    private readonly ILogger<UserRouteManager> _logger;

    public UserRouteManager(
        IRouteService routeService,
        IUserRouteService userRouteService,
        IUserService userService,
        ILogger<UserRouteManager> logger)
    {
        _routeService = routeService;
        _userRouteService = userRouteService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<UserRoute?> GetUserRouteAsync(int id)
    {
        return await _userRouteService.GetUserRoute(id);
    }

    public async Task<UserRoute?> CreateUserRouteAsync(CreateUserRouteRequest request)
    {
        // 1. Validate route exists
        var route = await _routeService.GetRoute(request.RouteId);
        if (route == null) throw new Exception($"Route {request.RouteId} not found.");

        // 2. Validate user
        var user = await _userService.GetUser(request.PersonId);
        if (user == null) throw new Exception($"User {request.PersonId} not found.");

        // 3. Convert user’s route points, if any
        var userRoutePoints = request.UserPoints
            .Select((p, i) => new UserRoutePoint
            {
                SequenceNumber = i,
                Point = new Point(new CoordinateZ(p.latitude, p.longitude, p.altitude ?? 0.0))
                {
                    SRID = 4326
                }
            })
            .ToList();

        // 4. Build the UserRoute entity
        var userRoute = new UserRoute
        {
            PersonId = request.PersonId,
            RouteId  = request.RouteId,
            DateTraveled = request.DateTraveled,
            Duration = request.Duration,
            Distance = request.Distance,
            ElevationGain = request.ElevationGain,
            DifficultyLevelId = request.DifficultyLevelId,
            RouteTypeId = request.RouteTypeId,
            UserRoutePoints = userRoutePoints
        };

        // 5. Save user route
        var createdUserRoute = await _userRouteService.CreateUserRoute(userRoute);

        // 6. Update user’s total distance (if you interpret "Distance" as how far they traveled)
        try
        {
            if (request.Distance.HasValue && request.Distance.Value > 0)
            {
                user.TotalDistance += request.Distance.Value;
                await _userService.UpdateUser(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update distance for user {UserId}", user.Id);
        }

        return createdUserRoute;
    }
}

}