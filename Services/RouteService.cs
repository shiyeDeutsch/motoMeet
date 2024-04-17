
using Microsoft.EntityFrameworkCore;
using motoMeet;


namespace motoMeet
{

    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetRoutes();
        Task<Route> GetRoute(int id);
        Task<Route> CreateRoute(Route route);
        Task AddPointsToRoute(int routeId, IEnumerable<RoutePoint> points);
        // Task AddTagsToRoute(int routeId, List<int>  tagIds);
    }

    public class RouteService : IRouteService
    {
        private readonly IRepository<Route> _routeRepository;
        private readonly IRepository<RoutePoint> _pointRepository;
      //  private readonly IRepository<RouteTag> _RouteTagRepository;
        private readonly IRepository<Tag> _TagRepository;

        public RouteService(IRepository<Route> routeRepository, IRepository<RoutePoint> pointRepository ,IRepository<Tag> tagRepository)
        {
            _routeRepository = routeRepository;
            _pointRepository = pointRepository;
        //    _RouteTagRepository = routeTagRepository;
            _TagRepository = tagRepository;
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {            return await _routeRepository.GetAllAsync();

          // return await _routeRepository.Find(SpecificationFactory.RouteWithAllRoutePoints());
        }

        public async Task<Route> GetRoute(int id)
        {
            return await _routeRepository.GetByIdAsync(id);
        }

        public async Task<Route> CreateRoute(Route route)
        {
            await _routeRepository.AddAsync(route);
            await _routeRepository.SaveAsync();
            await _routeRepository.ReloadAsync(route);
            return route;
        }

        public async Task AddPointsToRoute(int routeId, IEnumerable<RoutePoint> points)
        {
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null)
            {
                throw new Exception("Route not found");
            }



            // Add all points to the database in a single operation
            await _pointRepository.AddRangeAsync(points);
            await _pointRepository.SaveAsync();
        }
        // public async Task AddTagsToRoute(int routeId,List<int> tagIds)
        // {
        //     var route = await _routeRepository.GetByIdAsync(routeId);
        //     if (route == null)
        //     {
        //         throw new Exception("Route not found");
        //     }

        //     var tags = await _TagRepository.GetByIdsAsync(tagIds);
        //     List<RouteTag> routeTags = new List<RouteTag>();

        //     foreach (var tag in tags)
        //     {
        //         // Assuming RouteTag is the entity that represents the many-to-many relationship between Routes and Tags
        //         // and that it contains a RouteId and TagId to represent this relationship.
        //         // You also might want to check if the routeTag already exists to avoid duplicate entries.
        //         if (!routeTags.Any(rt => rt.RouteId == routeId && rt.TagId == tag.Id))
        //         {
        //             routeTags.Add(new RouteTag { RouteId = routeId, TagId = tag.Id });
        //         }
        //     }

        //     // AddRangeAsync adds each element of the enumerable to the context underlying the set
        //     // in one operation, which is more efficient than adding them one by one.
        //     await _RouteTagRepository.AddRangeAsync(routeTags);
        //     await _RouteTagRepository.SaveAsync();
        // }


    }
}