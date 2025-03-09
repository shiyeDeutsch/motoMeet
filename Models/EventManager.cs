using System;
using System.Collections.Generic;
using System.Linq;
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
        
        // New methods to support the client-side functionality
        Task<IEnumerable<Event>> GetEventsAsync(EventFilterRequest request);
        Task<bool> UpdateEventAsync(int eventId, UpdateEventRequest request);
        Task<bool> DeleteEventAsync(int eventId);
        Task<bool> CancelEventAsync(int eventId);
        Task<IEnumerable<Person>> GetEventParticipantsAsync(int eventId);
        Task<IEnumerable<Person>> GetPendingParticipantsAsync(int eventId);
        Task<bool> ApproveParticipantAsync(int eventId, int participantId);
        Task<bool> RejectParticipantAsync(int eventId, int participantId);
        Task<bool> RemoveParticipantAsync(int eventId, int participantId);
        Task<bool> LeaveEventAsync(int eventId, int personId);
        Task<bool> IsEventCreatorAsync(int eventId, int personId);
        Task<bool> IsEventParticipantAsync(int eventId, int personId);
        Task<string> GetEventCreatorNameAsync(int eventId);
        Task<int> GetEventParticipantCountAsync(int eventId);
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

            // If the request includes required items, add them
            if (request.RequiredItems != null && request.RequiredItems.Count > 0)
            {
                foreach (var itemRequest in request.RequiredItems)
                {
                    var eventItem = new EventItem
                    {
                        EventId = newEvent.Id,
                        ItemName = itemRequest.ItemName,
                        Description = itemRequest.Description,
                        IsAssigned = false
                    };
                    await _eventService.CreateEventItemAsync(eventItem);
                }
            }

            // If the request includes activities, add them
            if (request.EventActivities != null && request.EventActivities.Count > 0)
            {
                foreach (var activityRequest in request.EventActivities)
                {
                    var eventActivity = new EventActivity
                    {
                        EventId = newEvent.Id,
                        ActivityTypeId = activityRequest.ActivityTypeId
                    };
                    await _eventService.CreateEventActivityAsync(eventActivity);
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

        public async Task<IEnumerable<Event>> GetEventsAsync(EventFilterRequest request)
        {
            return await _eventService.GetEventsAsync(
                request.IsPublic,
                request.FromDate,
                request.ToDate,
                request.CreatorId
            );
        }

        public async Task<bool> UpdateEventAsync(int eventId, UpdateEventRequest request)
        {
            var existingEvent = await _eventService.GetEventByIdAsync(eventId);
            if (existingEvent == null)
                throw new Exception($"Event {eventId} not found.");
                
            // Check if the user is the creator
            if (existingEvent.CreatorId != request.RequestingUserId)
                throw new Exception("Only the event creator can update the event.");
                
            // Update the event properties
            existingEvent.Name = request.Name ?? existingEvent.Name;
            existingEvent.Description = request.Description ?? existingEvent.Description;
            
            if (request.IsPublic.HasValue)
                existingEvent.IsPublic = request.IsPublic.Value;
                
            if (request.RequiresApproval.HasValue)
                existingEvent.RequiresApproval = request.RequiresApproval.Value;
                
            if (request.StartDateTime.HasValue)
                existingEvent.StartDateTime = request.StartDateTime.Value;
                
            if (request.EndDateTime.HasValue)
                existingEvent.EndDateTime = request.EndDateTime.Value;
                
            return await _eventService.UpdateEventAsync(existingEvent);
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            return await _eventService.DeleteEventAsync(eventId);
        }

        public async Task<bool> CancelEventAsync(int eventId)
        {
            return await _eventService.CancelEventAsync(eventId);
        }

        public async Task<IEnumerable<Person>> GetEventParticipantsAsync(int eventId)
        {
            var participants = await _eventService.GetEventParticipantsAsync(eventId, true);
            var personIds = participants.Select(p => p.PersonId).ToList();
            return await _userService.GetUsersByIdsAsync(personIds);
        }

        public async Task<IEnumerable<Person>> GetPendingParticipantsAsync(int eventId)
        {
            var pendingParticipants = await _eventService.GetEventParticipantsAsync(eventId, false);
            var personIds = pendingParticipants.Select(p => p.PersonId).ToList();
            return await _userService.GetUsersByIdsAsync(personIds);
        }

        public async Task<bool> ApproveParticipantAsync(int eventId, int participantId)
        {
            return await _eventService.ApproveParticipantAsync(eventId, participantId);
        }

        public async Task<bool> RejectParticipantAsync(int eventId, int participantId)
        {
            return await _eventService.RejectParticipantAsync(eventId, participantId);
        }

        public async Task<bool> RemoveParticipantAsync(int eventId, int participantId)
        {
            return await _eventService.RemoveParticipantAsync(eventId, participantId);
        }

        public async Task<bool> LeaveEventAsync(int eventId, int personId)
        {
            var participant = await _eventService.GetEventParticipantByEventIdAndPersonId(eventId, personId);
            if (participant == null)
                return false;
                
            return await _eventService.RemoveParticipantAsync(eventId, participant.Id);
        }

        public async Task<bool> IsEventCreatorAsync(int eventId, int personId)
        {
            var ev = await _eventService.GetEventByIdAsync(eventId);
            return ev?.CreatorId == personId;
        }

        public async Task<bool> IsEventParticipantAsync(int eventId, int personId)
        {
            var participant = await _eventService.GetEventParticipantByEventIdAndPersonId(eventId, personId);
            return participant != null && participant.IsApproved;
        }

        public async Task<string> GetEventCreatorNameAsync(int eventId)
        {
            var ev = await _eventService.GetEventByIdAsync(eventId);
            if (ev == null)
                throw new Exception($"Event {eventId} not found.");
                
            var creator = await _userService.GetUser(ev.CreatorId);
            return creator?.UserName ?? "Unknown";
        }

        public async Task<int> GetEventParticipantCountAsync(int eventId)
        {
            var participants = await _eventService.GetEventParticipantsAsync(eventId, true);
            return participants.Count();
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
