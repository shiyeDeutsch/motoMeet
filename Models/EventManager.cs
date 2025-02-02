
using NetTopologySuite.Geometries;

namespace motoMeet
{
public interface IEventManager
{
    Task<Event> CreateEventAsync(CreateEventRequest request);
    Task<EventStage> AddStageAsync(int eventId, CreateEventStageRequest request);
    Task<EventParticipant> JoinEventAsync(int eventId, JoinEventRequest request);

    // For stage-level participation
    Task<EventStageParticipant> JoinStageAsync(int eventId, int stageId, JoinStageRequest request);

    // Possibly getters
    Task<Event> GetEventAsync(int eventId);
    Task<EventStage> GetStageAsync(int stageId);
}
public class EventManager : IEventManager
{
    private readonly IRepository<Event> _eventRepo;
    private readonly IRepository<EventStage> _stageRepo;
    private readonly IRepository<EventParticipant> _participantRepo;
    private readonly IRepository<EventStageParticipant> _stageParticipantRepo;
    private readonly IRepository<Person> _personRepo;
    private readonly IRepository<Route> _routeRepo;

    public EventManager(
        IRepository<Event> eventRepo,
        IRepository<EventStage> stageRepo,
        IRepository<EventParticipant> participantRepo,
        IRepository<EventStageParticipant> stageParticipantRepo,
        IRepository<Person> personRepo,
        IRepository<Route> routeRepo)
    {
        _eventRepo = eventRepo;
        _stageRepo = stageRepo;
        _participantRepo = participantRepo;
        _stageParticipantRepo = stageParticipantRepo;
        _personRepo = personRepo;
        _routeRepo = routeRepo;
    }

    public async Task<Event> CreateEventAsync(CreateEventRequest request)
    {
        // Validate creator
        var creator = await _personRepo.GetByIdAsync(request.CreatorId);
        if (creator == null)
            throw new Exception($"Creator with ID {request.CreatorId} not found.");

        // Build the event entity
        var newEvent = new Event
        {
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            RequiresApproval = request.RequiresApproval,
            CreatorId = request.CreatorId,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            GroupId = request.GroupId
        };

        // Save event
        await _eventRepo.AddAsync(newEvent);
        await _eventRepo.SaveAsync();

        // If the request includes stages, create them
        if (request.Stages != null && request.Stages.Count > 0)
        {
            foreach (var stageReq in request.Stages)
            {
                await AddStageInternal(newEvent.Id, stageReq);
            }
        }

        // Reload to include newly added stages if needed
        await _eventRepo.ReloadAsync(newEvent);
        return newEvent;
    }

    public async Task<EventStage> AddStageAsync(int eventId, CreateEventStageRequest request)
    {
        // Reuse an internal method for convenience
        return await AddStageInternal(eventId, request);
    }

    public async Task<EventParticipant> JoinEventAsync(int eventId, JoinEventRequest request)
    {
        var ev = await _eventRepo.GetByIdAsync(eventId);
        if (ev == null) throw new Exception($"Event {eventId} not found.");

        var person = await _personRepo.GetByIdAsync(request.PersonId);
        if (person == null) throw new Exception($"Person {request.PersonId} not found.");

        // Check if already participating
        var existing = await _participantRepo.FindFirstByExpressionAsync(
            p => p.EventId == eventId && p.PersonId == request.PersonId
        );
        if (existing != null)
            throw new Exception("User already joined this event.");

        var participant = new EventParticipant
        {
            EventId = eventId,
            PersonId = request.PersonId,
            JoinedOn = DateTime.UtcNow,
            IsApproved = !ev.RequiresApproval,
            IsActive = true
        };

        await _participantRepo.AddAsync(participant);
        await _participantRepo.SaveAsync();

        return participant;
    }

    public async Task<EventStageParticipant> JoinStageAsync(int eventId, int stageId, JoinStageRequest request)
    {
        // 1. Check stage
        var stage = await _stageRepo.GetByIdAsync(stageId);
        if (stage == null) throw new Exception($"Stage {stageId} not found.");
        if (stage.EventId != eventId)
            throw new Exception("Stage does not belong to the specified event.");

        // 2. Check if user is an event participant
        var participant = await _participantRepo.FindFirstByExpressionAsync(
            p => p.EventId == eventId && p.PersonId == request.PersonId
        );
        if (participant == null)
            throw new Exception("User is not a participant in this event. Must join event first.");

        // 3. Create an EventStageParticipant record
        var stageParticipant = new EventStageParticipant
        {
            EventStageId = stage.Id,
            EventParticipantId = participant.Id,
            StartedAt = request.StartedAt,
            IsCompleted = false
        };

        await _stageParticipantRepo.AddAsync(stageParticipant);
        await _stageParticipantRepo.SaveAsync();

        return stageParticipant;
    }

    public async Task<Event> GetEventAsync(int eventId)
    {
        var ev = await _eventRepo.GetByIdAsync(eventId);
        if (ev == null) throw new Exception($"Event {eventId} not found.");
        return ev;
    }

    public async Task<EventStage> GetStageAsync(int stageId)
    {
        var stage = await _stageRepo.GetByIdAsync(stageId);
        if (stage == null) throw new Exception($"EventStage {stageId} not found.");
        return stage;
    }

    // --------------------------
    //   Private Helper Methods
    // --------------------------
    private async Task<EventStage> AddStageInternal(int eventId, CreateEventStageRequest request)
    {
        // Check event
        var ev = await _eventRepo.GetByIdAsync(eventId);
        if (ev == null) throw new Exception($"Event {eventId} not found.");

        // If this stage references a route
        Route route = null;
        if (request.RouteId.HasValue)
        {
            route = await _routeRepo.GetByIdAsync(request.RouteId.Value);
            if (route == null)
                throw new Exception($"Route {request.RouteId} not found.");
        }

        // Build the location if relevant
        Point location = null;
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            location = new Point(new CoordinateZ(
                request.Latitude.Value,
                request.Longitude.Value,
                request.Altitude ?? 0.0))
            {
                SRID = 4326
            };
        }

        // Create stage
        var stage = new EventStage
        {
            EventId = ev.Id,
            Title = request.Title,
            Description = request.Description,
            StageStartTime = request.StageStartTime,
            RouteId = route?.Id,
            StageType = request.StageType,
            Location = location
        };

        await _stageRepo.AddAsync(stage);
        await _stageRepo.SaveAsync();
        return stage;
    }
}


}