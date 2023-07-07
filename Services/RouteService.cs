
using Microsoft.EntityFrameworkCore;
using motoMeet;


namespace motoMeet
{

    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetRoutes();
        Task<Route> GetRoute(int id);
        Task CreateRoute(Route route);
        Task AddPointToRoute(int routeId, RoutePoint point);
    }

    public class RouteService : IRouteService
    {
        private readonly IRepository<Route> _routeRepository;
        private readonly IRepository<RoutePoint> _pointRepository;

        public RouteService(IRepository<Route> routeRepository, IRepository<RoutePoint> pointRepository)
        {
            _routeRepository = routeRepository;
            _pointRepository = pointRepository;
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            return await _routeRepository.GetAllAsync();
        }

        public async Task<Route> GetRoute(int id)
        {
            return await _routeRepository.GetByIdAsync(id);
        }

        public async Task CreateRoute(Route route)
        {
            await _routeRepository.AddAsync(route);
        }

        public async Task AddPointToRoute(int routeId, RoutePoint point)
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null)
            {
                throw new Exception("Route not found");
            }

            point.RouteID = routeId;
            await _pointRepository.AddAsync(point);
        }
    }
}