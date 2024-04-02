using NetTopologySuite.Geometries;

namespace motoMeet
{
    public class Route : EntityBase
    {
        public int AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime EditOn { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int DifficultyLevelId { get; set; }
        public List<RoutesTypes> RouteTypes { get; set; }
        public double Length { get; set; }
        public TimeSpan Duration { get; set; }
        public double ElevationGain { get; set; }
        public double Rating { get; set; }
        public virtual DifficultyLevel DifficultyLevel { get; set; }
        public virtual List<RoutePoint> RoutePoints { get; set; }
        public virtual ICollection<RouteTag> RouteTags { get; set; }
        //Accessibility: String - Information about accessibility (e.g., "wheelchair accessible", "no motor vehicles").
        //Status: Enum - Current status of the route (e.g., Open, Closed, Under Maintenance).
    }
    public class NewRouteModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public RouteType RouteType { get; set; }
        public double Length { get; set; }
        public TimeSpan Duration { get; set; }
        // public DifficultyLevel DifficultyLevel { get; set; }
        public List<Point> RoutePoints { get; set; }
        public List<RouteTag> RouteTags { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class RoutePoint : EntityBase
    {
        public int ID { get; set; }
        public int RouteID { get; set; }
        public int SequenceNumber { get; set; }
        public Point Point { get; set; }
        //virtual   public Route Route { get; set; }
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

    }

    public class RoutesTypes : EntityBase
    {
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        public int RouteTypeId { get; set; }
        public virtual RouteType RouteType { get; set; }
    }

    public class Tag : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // The keyword or tag text

        // Navigation property for the many-to-many relationship
        public virtual ICollection<RouteTag> RouteTags { get; set; }
    }

    // Junction table for many-to-many relationship between Route and Tag
    public class RouteTag : EntityBase
    {
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}