public interface IUserRouteService
{
    Task<UserRoute> GetUserRoute(int id);
    Task<UserRoute> CreateUserRoute(UserRoute userRoute);
    // Possibly update, delete, etc.
}

public class UserRouteService : IUserRouteService
{
    private readonly IRepository<UserRoute> _userRouteRepo;
    // For userRoutePoints, etc.

    public UserRouteService(IRepository<UserRoute> userRouteRepo)
    {
        _userRouteRepo = userRouteRepo;
    }

    public async Task<UserRoute> GetUserRoute(int id)
    {
        return await _userRouteRepo.GetByIdAsync(id);
    }

    public async Task<UserRoute> CreateUserRoute(UserRoute userRoute)
    {
        await _userRouteRepo.AddAsync(userRoute);
        await _userRouteRepo.SaveAsync();
        await _userRouteRepo.ReloadAsync(userRoute);
        return userRoute;
    }
}
