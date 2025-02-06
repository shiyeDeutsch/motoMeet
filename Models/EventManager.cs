using System;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace motoMeet
{
    public interface IEventManager
    {
        Task<Event> CreateEventAsync(CreateEventRequest request);
        Task<EventStage> AddStageAsync(int eventId, CreateEventStageRequest request);
        Task<EventParticipant> JoinEventAsync(int eventId, JoinEventRequest request);
        Task<EventStageParticipant> JoinStageAsync(int eventId, int stageId, JoinStageRequest request);

        Task<Event> GetEventAsync(int eventId);
        Task<EventStage> GetStageAsync(int stageId);
    }

    public class EventManager : IEventManager
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;    // You already have this service
        private readonly IRouteService _routeService;  // So we can fetch routes if needed

        public EventManager(
            IEventService eventService,
            IUserService userService,
            IRouteService routeService)
        {
            _eventService = eventService;
            _userService  = userService;
            _routeService = routeService;
        }

        public async Task<Event> CreateEventAsync(CreateEventRequest request)
        {
            // Validate creator via IUserService
            var creator = await _userService.GetUser(request.CreatorId);
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

            // Create the event in DB
            await _eventService.CreateEventAsync(newEvent);

            // If the request includes stages, create them
            if (request.Stages != null && request.Stages.Count > 0)
            {
                foreach (var stageReq in request.Stages)
                {
                    await AddStageInternal(newEvent.Id, stageReq);
                }
            }

            // Reload the event to bring in newly added stages if needed
            await _eventService.ReloadEventAsync(newEvent);
            return newEvent;
        }

        public async Task<EventStage> AddStageAsync(int eventId, CreateEventStageRequest request)
        {
            return await AddStageInternal(eventId, request);
        }

        public async Task<EventParticipant> JoinEventAsync(int eventId, JoinEventRequest request)
        {
            // Check event
            var ev = await _eventService.GetEventByIdAsync(eventId);
            if (ev == null) throw new Exception($"Event {eventId} not found.");

            // Check person
            var person = await _userService.GetUser(request.PersonId);
            if (person == null) throw new Exception($"Person {request.PersonId} not found.");

            // Check if already participating
            var existing = await _eventService.GetEventParticipantByEventIdAndPersonId(eventId, request.PersonId);
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

            // Create participant in DB
            await _eventService.CreateEventParticipantAsync(participant);
            return participant;
        }

        public async Task<EventStageParticipant> JoinStageAsync(int eventId, int stageId, JoinStageRequest request)
        {
            // 1. Check stage
            var stage = await _eventService.GetStageByIdAsync(stageId);
            if (stage == null) throw new Exception($"Stage {stageId} not found.");
            if (stage.EventId != eventId)
                throw new Exception("Stage does not belong to the specified event.");

            // 2. Check if user is an event participant
            var participant = await _eventService.GetEventParticipantByEventIdAndPersonId(eventId, request.PersonId);
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

            // Create in DB
            await _eventService.CreateEventStageParticipantAsync(stageParticipant);
            return stageParticipant;
        }

        public async Task<Event> GetEventAsync(int eventId)
        {
            var ev = await _eventService.GetEventByIdAsync(eventId);
            if (ev == null) throw new Exception($"Event {eventId} not found.");
            return ev;
        }

        public async Task<EventStage> GetStageAsync(int stageId)
        {
            var stage = await _eventService.GetStageByIdAsync(stageId);
            if (stage == null) throw new Exception($"EventStage {stageId} not found.");
            return stage;
        }

        // -----------------------------------
        //  Private Helper for adding stages
        // -----------------------------------
        private async Task<EventStage> AddStageInternal(int eventId, CreateEventStageRequest request)
        {
            // Check event
            var ev = await _eventService.GetEventByIdAsync(eventId);
            if (ev == null) throw new Exception($"Event {eventId} not found.");

            // If this stage references a route
            Route route = null;
            if (request.RouteId.HasValue)
            {
                // Use RouteService to fetch
                route = await _routeService.GetRoute(request.RouteId.Value);
                if (route == null)
                    throw new Exception($"Route {request.RouteId} not found.");
            }

            // Build the location if relevant
            Point location = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                location = new Point(
                    new NetTopologySuite.Geometries.CoordinateZ(
                        request.Latitude.Value,
                        request.Longitude.Value,
                        request.Altitude ?? 0.0
                    ))
                {
                    SRID = 4326
                };
            }

            // Create stage
            var stage = new EventStage
            {
                EventId        = ev.Id,
                Title          = request.Title,
                Description    = request.Description,
                StageStartTime = request.StageStartTime,
                RouteId        = route?.Id,
                StageType      = request.StageType,
                Location       = location
            };

            // Save stage via IEventService
            await _eventService.CreateEventStageAsync(stage);
            return stage;
        }
    }
}
