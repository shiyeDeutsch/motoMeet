
using motoMeet;

namespace motoMeet
{
    public class Event : EntityBase
    {
        public int Id { get; set; }

        // Basic info
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        // The “main route” associated with this event
        // or possibly multiple routes or "stages"
        public int? RouteId { get; set; }
        public virtual Route Route { get; set; }

        // Creator of the event
        public int CreatorId { get; set; }
        public virtual Person Creator { get; set; }

        // Who is participating (and possibly how)
        public virtual ICollection<EventParticipant> Participants { get; set; }
            = new List<EventParticipant>();

        // Optional: Additional fields for stages, items, etc.
    }
    public class EventParticipant : EntityBase
    {
        public int Id { get; set; }

        // Link to the event
        public int EventId { get; set; }
        public virtual Event Event { get; set; }

        // Link to the person
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        // Possible status fields (joined? invited? role? etc.)
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        // etc.

        // If each participant has their own route tracking 
        // we can link it here or just rely on a separate `UserRoute`.
        // For example, we could do a "UserRouteId" if you want each participant’s usage:
        public int? UserRouteId { get; set; }
        public virtual UserRoute UserRoute { get; set; }
    }

}