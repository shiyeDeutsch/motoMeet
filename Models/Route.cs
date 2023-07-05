using NetTopologySuite.Geometries;

namespace motoMeet
{
    public class Route
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int DifficultyLevelID { get; set; }

        // Navigation property
        public DifficultyLevel DifficultyLevel { get; set; }

        // Navigation property for RoutePoints
        public List<RoutePoint> RoutePoints { get; set; }
    }

    public class RoutePoint
    {
        public int ID { get; set; }
        public int RouteID { get; set; }
        public int SequenceNumber { get; set; }
         public Point Point { get; set; }
        public decimal Elevation { get; set; }

        // Navigation property
        public Route Route { get; set; }
    }

    public class DifficultyLevel
    {
        public int ID { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }

        // Navigation property for Routes
        public List<Route> Routes { get; set; }
    }

}
