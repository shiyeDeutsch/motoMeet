

using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace motoMeet
{
   public class Event : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public bool IsPublic { get; set; }
    public bool RequiresApproval { get; set; }

 
    public int CreatorId { get; set; }
    public virtual Person Creator { get; set; }

    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }

    // Link to group if event is created inside a group
    public int? GroupId { get; set; }
    public virtual Group Group { get; set; }

    // Items needed, e.g. "Flashlight, rope..."
    public virtual ICollection<EventItem> RequiredItems { get; set; }
      = new List<EventItem>();

    // The participants pivot
    public virtual ICollection<EventParticipant> Participants { get; set; }
      = new List<EventParticipant>();

    // Possibly "stages" of the event
    public virtual ICollection<EventStage> Stages { get; set; }
      = new List<EventStage>();

          public virtual ICollection<EventActivity> EventActivities { get; set; }
        = new List<EventActivity>();
}
public class EventStage : EntityBase
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public virtual Event Event { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }

    public DateTime? StageStartTime { get; set; }
    public DateTime? StageEndTime { get; set; }
    public int? RouteId { get; set; }
    public virtual Route Route { get; set; }

    public EventStageType StageType { get; set; }
    public Point? Location { get; set; }

    // For clarity:
    public virtual ICollection<EventStageParticipant> StageParticipants { get; set; }
      = new List<EventStageParticipant>();
}

public class EventParticipant : EntityBase
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public virtual Event Event { get; set; }

    public int PersonId { get; set; }
    public virtual Person Person { get; set; }

    public bool IsApproved { get; set; }
    public bool IsActive { get; set; }
    public DateTime JoinedOn { get; set; }

    // List of stages the participant is engaged in
    public virtual ICollection<EventStageParticipant> StageParticipants { get; set; }
      = new List<EventStageParticipant>();
}

public class EventStageParticipant : EntityBase
{
    public int Id { get; set; }

    // Link to the "stage" 
    public int EventStageId { get; set; }
    public virtual EventStage EventStage { get; set; }

    // Link to the participant (the user in the event)
    public int EventParticipantId { get; set; }
    public virtual EventParticipant EventParticipant { get; set; }

    // If the user logs a personal usage (UserRoute) for the route associated with this stage
    public int? UserRouteId { get; set; }
    public virtual UserRoute UserRoute { get; set; }

    // Additional details for stage-level participation
    // e.g. times, statuses, did they skip, etc.
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public bool IsCompleted { get; set; }
}


public class ActivityType : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; } 
        // e.g. "Hiking", "Biking", "Camping", "JeepOffroad", ...

    // Optionally more fields, like category or short descriptions
    // public string Category { get; set; }
    // public string Description { get; set; }

    // If you want a back-reference to events/groups, you can do it,
    // but typically you’ll just configure pivot relationships.
    public virtual ICollection<EventActivity> EventActivities { get; set; } 
        = new List<EventActivity>();
    public virtual ICollection<GroupActivity> GroupActivities { get; set; } 
        = new List<GroupActivity>();
}

public class EventActivity
{
    // Composite PK: (EventId, ActivityTypeId)
    public int EventId { get; set; }
    public virtual Event Event { get; set; }

    public int ActivityTypeId { get; set; }
    public virtual ActivityType ActivityType { get; set; }
}


    public class EventItem : EntityBase
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public virtual Event Event { get; set; }

    public string ItemName { get; set; }
    public string? Description { get; set; }
    public bool IsAssigned { get; set; } 
}

 
public enum EventStageType
{
    RouteSegment,    // A stage that references a Route
    MeetingPoint,    // A stage with a single coordinate or place
    OvernightStop,   // E.g. camping, lodging
    LunchStop,       // Another specific stop type
    Activity         // A custom activity that doesn’t necessarily have a route
}


public class PersonFollow : EntityBase
{
    public int Id { get; set; }

    /// The person who follows someone
    public int FollowerId { get; set; }
    public virtual Person Follower { get; set; }

    /// The person being followed
    public int FollowingId { get; set; }
    public virtual Person Following { get; set; }

    public DateTime FollowedOn { get; set; }

    // Possibly a status (“requested,” “accepted”) if you want an approval flow
    // public bool IsApproved { get; set; }
}
public class Group : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

  // Location can be a string or a geometry
    public string? Location { get; set; }

    // Privacy settings: public, private, or hidden
    public bool IsPublic { get; set; }
    public bool IsApprovalRequired { get; set; } 
      // e.g. join requests must be approved by an admin

    // The person who created this group
    public int CreatorId { get; set; }
    public virtual Person Creator { get; set; }

    // Members (pivot table)
    public virtual ICollection<GroupMember> Members { get; set; } 
      = new List<GroupMember>();

    // E.g. group posts or events
    public virtual ICollection<GroupPost> Posts { get; set; } 
      = new List<GroupPost>();
    
    public virtual ICollection<Event> Events { get; set; } 
      = new List<Event>();

       public virtual ICollection<GroupActivity> GroupActivities { get; set; }
        = new List<GroupActivity>();

}
public class GroupMember : EntityBase
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public virtual Group Group { get; set; }

    public int PersonId { get; set; }
    public virtual Person Person { get; set; }

    // Roles / Permissions
    public bool IsAdmin { get; set; }
    public bool CanPost { get; set; }
    public bool IsApproved { get; set; } 
      // if group requires approval

    public DateTime JoinedOn { get; set; }
}
public class GroupActivity
{
    // Composite PK: (GroupId, ActivityTypeId)
    public int GroupId { get; set; }
    public virtual Group Group { get; set; }

    public int ActivityTypeId { get; set; }
    public virtual ActivityType ActivityType { get; set; }

    // Optional extra fields like IsPrimary.
}

public class GroupPost : EntityBase
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public virtual Group Group { get; set; }

    public int AuthorId { get; set; }
    public virtual Person Author { get; set; }

    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }

    // Possibly a collection of attachments, likes, or comments
    public virtual ICollection<GroupPostAttachment> Attachments { get; set; } 
      = new List<GroupPostAttachment>();

    public virtual ICollection<GroupPostComment> Comments { get; set; }
      = new List<GroupPostComment>();

      [NotMapped]
    public virtual ICollection<Reaction> Reactions { get; set; }
        = new List<Reaction>();
}
public class GroupPostComment : EntityBase
{
    public int Id { get; set; }
    public int GroupPostId { get; set; }
    public virtual GroupPost GroupPost { get; set; }

    public int AuthorId { get; set; }
    public virtual Person Author { get; set; }

    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class GroupPostAttachment : EntityBase
{
    public int Id { get; set; }
    public int GroupPostId { get; set; }
    public virtual GroupPost GroupPost { get; set; }

    public string FileUrl { get; set; }
    public string AttachmentType { get; set; } // e.g. image, video, doc
    public DateTime UploadedOn { get; set; }
}

}