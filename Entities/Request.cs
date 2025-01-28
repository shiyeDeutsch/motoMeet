using motoMeet.Manager;

namespace motoMeet
{
    public class CreateRouteRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    // Person who is creating/publishing the route. 
    // (Could be "creatorId" or "addedBy".)
    public int CreatorId { get; set; }

    // Points
    public GeoPoint StartPoint { get; set; }
    public GeoPoint EndPoint { get; set; }
    public List<GeoPoint> RoutePoints { get; set; }

    public double Length { get; set; }
    public TimeSpan Duration { get; set; }

    public List<int> RouteTagsIds { get; set; } = new List<int>();

    // Possibly also official difficulty or route type if you want them set here:
    public int? DifficultyLevelId { get; set; }
    public int? RouteTypeId { get; set; }
}
public class CreateUserRouteRequest
{
    public int PersonId { get; set; }     // which user
    public int RouteId { get; set; }      // referencing existing official route

    // The user’s actual usage
    public DateTime DateTraveled { get; set; } = DateTime.UtcNow;
    public TimeSpan? Duration { get; set; }
    public double? Distance { get; set; }
    public double? ElevationGain { get; set; }

    public int? DifficultyLevelId { get; set; }  // user-chosen difficulty
    public int? RouteTypeId { get; set; }        // user-chosen route type

    public List<GeoPoint> UserPoints { get; set; } 
        = new List<GeoPoint>();  // user’s actual GPS track if needed
}

}
