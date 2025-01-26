using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace motoMeet
{

    public class Route : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Person Person { get; set; }
        public int PersonId { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime EditOn { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        // Spatial data (NetTopologySuite type, for example)
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        // Official RouteType
        public int? RouteTypeId { get; set; }
        public virtual RouteType RouteType { get; set; }

        // Official DifficultyLevel
        public int? DifficultyLevelId { get; set; }
        public virtual DifficultyLevel DifficultyLevel { get; set; }

        public double Length { get; set; }
        public TimeSpan Duration { get; set; }
        public double ElevationGain { get; set; }
        public double Rating { get; set; }
        public bool isLoop { get; set; }

        public string? Country { get; set; }
        public string? Region { get; set; }

        // One route can have multiple route points
        public virtual ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();

        // One route can have multiple reviews
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Many-to-many with Tag
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        // One route can be logged by many users in the UserRoute table
        public virtual ICollection<UserRoute> UserRoutes { get; set; } = new List<UserRoute>();

        public virtual ICollection<PointOfInterest> PointsOfInterest { get; set; }
            = new List<PointOfInterest>();

        // Example computed properties (not mapped) to gather distinct user-submitted difficulties/types
        [NotMapped]
        public IEnumerable<DifficultyLevel> DifficultyLevelsUsedByTravelers
            => UserRoutes
                .Where(ur => ur.DifficultyLevel != null)
                .Select(ur => ur.DifficultyLevel)
                .Distinct();

        [NotMapped]
        public IEnumerable<RouteType> RouteTypesUsedByTravelers
            => UserRoutes
                .Where(ur => ur.RouteType != null)
                .Select(ur => ur.RouteType)
                .Distinct();
    }


    public class UserRoute : EntityBase
    {
        public int Id { get; set; }

        // Links to the user (Person)
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        // Links to the route
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        // The user’s chosen difficulty (if they differ from the official route)
        public int? DifficultyLevelId { get; set; }
        public virtual DifficultyLevel DifficultyLevel { get; set; }

        // The user’s chosen route type (if they differ from the official route)
        public int? RouteTypeId { get; set; }
        public virtual RouteType RouteType { get; set; }

        // Additional per-travel or per-usage data
        public DateTime DateTraveled { get; set; }
        public TimeSpan? Duration { get; set; }      // user’s time on the route
        public double? Distance { get; set; }        // user’s measured distance
        public double? ElevationGain { get; set; }   // user’s measured elevation gain


        public virtual ICollection<UserRoutePoint> UserRoutePoints { get; set; }
    }
    public class RoutePoint : EntityBase
    {
        public int Id { get; set; }

        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        public int SequenceNumber { get; set; }

        // Another spatial type (e.g. NetTopologySuite Point)
        public Point Point { get; set; }
    }
    public class UserRoutePoint
    {
        public int Id { get; set; }
        public int UserRouteId { get; set; }
        public UserRoute UserRoute { get; set; }
        public int SequenceNumber { get; set; }
        public Point Point { get; set; }
    }
    public class PointOfInterest : EntityBase
    {
        public int Id { get; set; }

        // Link to the Route (official route)
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        // Where is this POI located?
        public Point Location { get; set; }

        // Additional metadata
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Enum describing the type (waterfall, cave, etc.)
        public WaypointType WaypointType { get; set; }
    }
    public class DifficultyLevel : EntityBase
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }

        // Routes that have this difficulty officially
        public virtual ICollection<Route> OfficialRoutes { get; set; } = new List<Route>();

        // UserRoute logs that used this difficulty
        public virtual ICollection<UserRoute> UserRoutes { get; set; } = new List<UserRoute>();
    }
    public class Review : EntityBase
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
    }
    public class RouteType : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Routes that officially have this route type
        public virtual ICollection<Route> OfficialRoutes { get; set; } = new List<Route>();

        // UserRoute logs that used this route type
        public virtual ICollection<UserRoute> UserRoutes { get; set; } = new List<UserRoute>();
    }
    public class Tag : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Many-to-many: A Tag can be used for many Routes, and a Route can have many Tags
        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    }
    public class GeoPoint
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double? altitude { get; set; }

    }
    public class NewRouteModel
    {

        // public CoordinateZ coordinateZ = new NetTopologySuite.Geometries.CoordinateZ(x: 1.0, y: 2.0, z: 3.0);
        // Point pointFromCoordinateZ  => new NetTopologySuite.Geometries.Point(coordinateZ);

        public string Name { get; set; }
        public string Description { get; set; }
        public int AddedBy { get; set; }

        // New property for incoming data as an array
        public GeoPoint StartPointArray { get; set; }

        // Convert StartPointArray to Point on-the-fly
        public Point? StartPoint => (StartPointArray != null) ? new Point(new CoordinateZ(StartPointArray.latitude, StartPointArray.longitude, StartPointArray.altitude ?? 0.0)) { SRID = 4326 } : null;

        public GeoPoint EndPointArray { get; set; }
        public GeoPoint[] RoutePointsArray { get; set; }
        // Convert EndPointArray to Point on-the-fly
        public Point EndPoint => EndPointArray != null ? new Point(new CoordinateZ(EndPointArray.latitude, EndPointArray.longitude, EndPointArray.altitude ?? 0.0)) { SRID = 4326 } : null;

        public RouteType RouteType { get; set; }
        public double Length { get; set; }
        public TimeSpan Duration { get; set; }
        // public DifficultyLevel DifficultyLevel { get; set; }
        public List<Point> RoutePoints => RoutePointsArray?.Select(p => new Point(new CoordinateZ(p.latitude, p.longitude, p.altitude ?? 0.0)) { SRID = 4326 }).ToList();
        public List<int> RouteTagsIds { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
    public enum WaypointType
    {
        // Natural
        Lake,
        Cliff,
        Waterfall,
        WaterSpring,
        River,
        MountainPeak,
        Forest,
        Meadow,
        Cave,
        Valley,
        Beach,
        Glacier,
        Volcano,

        // Informative
        HistoricalSite,
        VisitorCenter,
        Viewpoint,
        Museum,
        CulturalSite,
        EducationalTrail,
        ParkOffice,

        // Warnings
        SteepDrop,
        SlipperyPath,
        HighTide,
        WildlifeSighting,
        FloodingArea,
        Rockfall,
        RestrictedArea
    }

}