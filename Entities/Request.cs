using System.ComponentModel.DataAnnotations;
 
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
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool RequiresApproval { get; set; } = false;
        [Required]
        public int CreatorId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? GroupId { get; set; }
        public List<CreateEventStageRequest> Stages { get; set; }
        public List<CreateRequiredItemRequest> RequiredItems { get; set; }
        public List<CreateEventActivityRequest> EventActivities { get; set; }
    }

    public class CreateEventStageRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StageStartTime { get; set; }
        public int? RouteId { get; set; }
        public string StageType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Altitude { get; set; }
    }

    public class CreateRequiredItemRequest
    {
        [Required]
        public string ItemName { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; } = true;
    }

    public class CreateEventActivityRequest
    {
        [Required]
        public int ActivityTypeId { get; set; }
    }

    public class JoinEventRequest
    {
        [Required]
        public int PersonId { get; set; }
    }

    public class StageParticipationRequest
    {
        public int EventId { get; set; }
        public int StageId { get; set; }
        public int PersonId { get; set; }
    }

    public class JoinStageRequest
    {
        [Required]
        public int PersonId { get; set; }
        public DateTime? StartedAt { get; set; }
    }

    public class UpdateEventRequest
    {
        [Required]
        public int RequestingUserId { get; set; } // To verify creator
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPublic { get; set; }
        public bool? RequiresApproval { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }

    public class EventFilterRequest
    {
        public bool? IsPublic { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CreatorId { get; set; }
        public int? EventTypeId { get; set; }
        public bool? IncludeCancelled { get; set; }
    }

    public class ApproveParticipantRequest
    {
        [Required]
        public int ParticipantId { get; set; }
    }
}
