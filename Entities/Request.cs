

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
public class CreateEventRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public bool IsPublic { get; set; }
    public bool RequiresApproval { get; set; }
    
    public int CreatorId { get; set; } // the user who creates the event
    
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }

    public int? GroupId { get; set; }

    // Optional list of stages to create immediately
    public List<CreateEventStageRequest>? Stages { get; set; } 
        = new List<CreateEventStageRequest>();
}
public class CreateEventStageRequest
{
    public string Title { get; set; }
    public string? Description { get; set; }
    
    public DateTime? StageStartTime { get; set; }

    // If this stage references an existing Route (RouteId)
    public int? RouteId { get; set; }

    // The stage type (RouteSegment, MeetingPoint, etc.)
    public EventStageType StageType { get; set; }

    // If this is a meeting point, you can store lat/lon
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Altitude { get; set; }
}
public class JoinEventRequest
{
    public int PersonId { get; set; }
    // Additional data if needed, e.g. a custom message
}

public class StageParticipationRequest
{
    public int EventId { get; set; }
    public int StageId { get; set; }
    public int PersonId { get; set; }
}

public class JoinStageRequest
{
    public int PersonId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    // Possibly other fields if needed
}

}
