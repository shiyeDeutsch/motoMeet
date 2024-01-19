using NetTopologySuite.Geometries;

namespace motoMeet
{
    public class Route
    {
        public int AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime EditOn { get; set; }
        public int Id  { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int DifficultyLevelID { get; set; }
        public List<RouteType> RouteTypes { get; set; }
        public double Length  {get;set;} 
        public TimeSpan Duration { get; set; }
        public double ElevationGain { get; set; }
        public double Rating  { get; set; }
        public virtual   DifficultyLevel DifficultyLevel { get; set; }
        public virtual   List<RoutePoint> RoutePoints { get; set; }
        public virtual ICollection<RouteTag> RouteTags { get; set; }
        //Accessibility: String - Information about accessibility (e.g., "wheelchair accessible", "no motor vehicles").
        //Status: Enum - Current status of the route (e.g., Open, Closed, Under Maintenance).
    }

    public class RoutePoint
    {
        public int ID { get; set; }
        public int RouteID { get; set; }
        public int SequenceNumber { get; set; }
        public Point Point { get; set; }
        //virtual   public Route Route { get; set; }
    }

    public class DifficultyLevel
    {
        public int ID { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
    virtual    public List<Route> Routes { get; set; }
    }

}
public class Review
{
    public int Id { get; set; } // Unique identifier for the review
    public string Username { get; set; } // Username of the reviewer
    public double Rating { get; set; } // Rating given by the reviewer
    public string Comment { get; set; } // Text of the review
    public DateTime Date { get; set; } 
    public int RouteId { get; set; }    
    public virtual Route Route { get; set; } // Navigation property for the Route

    }
public enum RouteType
{
    Hiking,
    MountainBiking,
    Running,
    RoadBiking,
    Backpacking,
    Walking,
    OffRoadDriving,
    ScenicDriving,
    BikeTouring,
    Snowshoeing,
    CrossCountrySkiing,
    Skiing,
    PaddleSports,
    Camping,
    Fishing,
    BirdWatching,
    HorsebackRiding,
    RockClimbing,
    ViaFerrata
    // You can add more activities as your app evolves
}
public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } 
    // The keyword or tag text

    // Navigation property for the many-to-many relationship
    public virtual ICollection<RouteTag> RouteTags { get; set; }
}

// Junction table for many-to-many relationship between Route and Tag
public class RouteTag
{
    public int RouteId { get; set; }
    public Route Route { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}

