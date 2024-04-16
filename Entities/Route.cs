using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace motoMeet
{
    public class Route : EntityBase
    {
        public int AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime EditOn { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int? DifficultyLevelId { get; set; }
        //   public List<RoutesTypes>? RouteTypes { get; set; }
        public RouteType? RouteTypes { get; set; }
        public int? RouteTypeId { get; set; }
        public double Length { get; set; }
        public TimeSpan Duration { get; set; }
        public double ElevationGain { get; set; }
        public double Rating { get; set; }
        // public virtual DifficultyLevel ?DifficultyLevel { get; set; }
        public virtual ICollection<RoutePoint> RoutePoints { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }

        //Accessibility: String - Information about accessibility (e.g., "wheelchair accessible", "no motor vehicles").
        //Status: Enum - Current status of the route (e.g., Open, Closed, Under Maintenance).
    }
    // public class NewRouteModel
    // {
    //     public string Name { get; set; }
    //     public string Description { get; set; }
    //     public Point StartPoint { get; set; }
    //     public Point EndPoint { get; set; }
    //     public RouteType RouteType { get; set; }
    //     public double Length { get; set; }
    //     public TimeSpan Duration { get; set; }
    //     // public DifficultyLevel DifficultyLevel { get; set; }
    //     public List<Point> RoutePoints { get; set; }
    //     public List<RouteTag> RouteTags { get; set; }
    //     public bool IsActive { get; set; }
    //     public bool IsCompleted { get; set; }
    //     public DateTime StartDate { get; set; }
    //     public DateTime? EndDate { get; set; }
    // }

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
    public class GeoPoint
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double? altitude { get; set; }

    }
    public class RoutePoint : EntityBase
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int SequenceNumber { get; set; }
        public Point Point { get; set; }
         public virtual  Route Route { get; set; }
    }
    public class DifficultyLevel : EntityBase
    {
        public int ID { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        virtual public List<Route> Routes { get; set; }
    }
    public class Review : EntityBase
    {
        public int Id { get; set; } // Unique identifier for the review
        public string Username { get; set; } // Username of the reviewer
        public double Rating { get; set; } // Rating given by the reviewer
        public string Comment { get; set; } // Text of the review
        public DateTime Date { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; } // Navigation property for the Route

    }

    public class RouteType : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Route ?Route { get; set; }

    }

    // public class RoutesTypes : EntityBase
    // {
    //     public int RouteId { get; set; }
    //     public virtual Route Route { get; set; }

    //     public int RouteTypeId { get; set; }
    //     public virtual RouteType RouteType { get; set; }
    // }

    public class Tag : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // The keyword or tag text

        // Navigation property for the many-to-many relationship
        public ICollection<Route>? Routes { get; set; }
        //  public ICollection<RouteTag> RouteTags { get; set; } = new List<RouteTag>();

    }

    // Junction table for many-to-many relationship between Route and Tag
    // public class RouteTag : EntityBase
    // {
    //     public int RouteId { get; set; }
    //     public virtual Route? Route { get; set; }

    //     public int TagId { get; set; }
    //     public virtual Tag? Tag { get; set; }
    // }

}