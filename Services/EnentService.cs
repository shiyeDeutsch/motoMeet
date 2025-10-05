using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace motoMeet
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(Event ev);
        Task<EventStage> CreateEventStageAsync(EventStage stage);
        Task<EventParticipant> CreateEventParticipantAsync(EventParticipant participant);
        Task<EventStageParticipant> CreateEventStageParticipantAsync(EventStageParticipant stageParticipant);
        Task<EventItem> CreateEventItemAsync(EventItem item);
        Task<EventActivity> CreateEventActivityAsync(EventActivity activity);

        Task<Event> GetEventByIdAsync(int eventId);
        Task<EventStage> GetStageByIdAsync(int stageId);

        /// <summary>
        /// Returns the EventParticipant if a person is participating in a given event, otherwise null.
        /// </summary>
        Task<EventParticipant> GetEventParticipantByEventIdAndPersonId(int eventId, int personId);

        /// <summary>
        /// Reloads the Event entity to include newly added child objects (e.g., Stages).
        /// </summary>
        Task ReloadEventAsync(Event ev);
        
        /// <summary>
        /// Updates an existing event.
        /// </summary>
        Task<bool> UpdateEventAsync(Event ev);
        
        /// <summary>
        /// Deletes an event.
        /// </summary>
        Task<bool> DeleteEventAsync(int eventId);
        
        /// <summary>
        /// Gets all events with optional filtering.
        /// </summary>
        Task<IEnumerable<Event>> GetEventsAsync(
            bool? isPublic = null, 
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            int? creatorId = null);
        
        /// <summary>
        /// Gets all participants for an event.
        /// </summary>
        Task<IEnumerable<EventParticipant>> GetEventParticipantsAsync(int eventId, bool? isApproved = null);
        
        /// <summary>
        /// Approves a participant for an event.
        /// </summary>
        Task<bool> ApproveParticipantAsync(int eventId, int participantId);
        
        /// <summary>
        /// Rejects a participant for an event.
        /// </summary>
        Task<bool> RejectParticipantAsync(int eventId, int participantId);
        
        /// <summary>
        /// Removes a participant from an event.
        /// </summary>
        Task<bool> RemoveParticipantAsync(int eventId, int participantId);
        
        /// <summary>
        /// Cancels an event.
        /// </summary>
        Task<bool> CancelEventAsync(int eventId);
    }

    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<EventStage> _stageRepo;
        private readonly IRepository<EventParticipant> _participantRepo;
        private readonly IRepository<EventStageParticipant> _stageParticipantRepo;
        private readonly IRepository<EventItem> _eventItemRepo;
        private readonly IRepository<EventActivity> _eventActivityRepo;

        public EventService(
            IRepository<Event> eventRepo,
            IRepository<EventStage> stageRepo,
            IRepository<EventParticipant> participantRepo,
            IRepository<EventStageParticipant> stageParticipantRepo,
            IRepository<EventItem> eventItemRepo,
            IRepository<EventActivity> eventActivityRepo)
        {
            _eventRepo = eventRepo;
            _stageRepo = stageRepo;
            _participantRepo = participantRepo;
            _stageParticipantRepo = stageParticipantRepo;
            _eventItemRepo = eventItemRepo;
            _eventActivityRepo = eventActivityRepo;
        }

        public async Task<Event> CreateEventAsync(Event ev)
        {
            await _eventRepo.AddAsync(ev);
            await _eventRepo.SaveAsync();
            return ev;
        }

        public async Task<EventStage> CreateEventStageAsync(EventStage stage)
        {
            await _stageRepo.AddAsync(stage);
            await _stageRepo.SaveAsync();
            return stage;
        }

        public async Task<EventParticipant> CreateEventParticipantAsync(EventParticipant participant)
        {
            await _participantRepo.AddAsync(participant);
            await _participantRepo.SaveAsync();
            return participant;
        }

        public async Task<EventStageParticipant> CreateEventStageParticipantAsync(EventStageParticipant stageParticipant)
        {
            await _stageParticipantRepo.AddAsync(stageParticipant);
            await _stageParticipantRepo.SaveAsync();
            return stageParticipant;
        }
        
        public async Task<EventItem> CreateEventItemAsync(EventItem item)
        {
            await _eventItemRepo.AddAsync(item);
            await _eventItemRepo.SaveAsync();
            return item;
        }
        
        public async Task<EventActivity> CreateEventActivityAsync(EventActivity activity)
        {
            await _eventActivityRepo.AddAsync(activity);
            await _eventActivityRepo.SaveAsync();
            return activity;
        }

        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await _eventRepo.GetByIdAsync(eventId);
        }

        public async Task<EventStage> GetStageByIdAsync(int stageId)
        {
            return await _stageRepo.GetByIdAsync(stageId);
        }

        public async Task<EventParticipant> GetEventParticipantByEventIdAndPersonId(int eventId, int personId)
        {
            return await _participantRepo.FindFirstByExpressionAsync(
                p => p.EventId == eventId && p.PersonId == personId
            );
        }

        public async Task ReloadEventAsync(Event ev)
        {
            await _eventRepo.ReloadAsync(ev);
        }
        
        public async Task<bool> UpdateEventAsync(Event ev)
        {
            _eventRepo.Update(ev);
            await _eventRepo.SaveAsync();
            return true;
        }
        
        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var ev = await _eventRepo.GetByIdAsync(eventId);
            if (ev == null)
                return false;
                
            _eventRepo.Delete(ev);
            await _eventRepo.SaveAsync();
            return true;
        }
        
        public async Task<IEnumerable<Event>> GetEventsAsync(
            bool? isPublic = null, 
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            int? creatorId = null)
        {
            // Use specification pattern to build the query
            var specification = new EventSpecification();
            
            if (isPublic.HasValue)
                specification.ByIsPublic(isPublic.Value);
                
            if (fromDate.HasValue)
                specification.ByStartDateFrom(fromDate.Value);
                
            if (toDate.HasValue)
                specification.ByStartDateTo(toDate.Value);
                
            if (creatorId.HasValue)
                specification.ByCreator(creatorId.Value);
                
            // Add includes for related entities
            specification.IncludeEventStages()
                         .IncludeRequiredItems()
                         .IncludeEventActivities()
                         .IncludeEventParticipants();
            
            return await _eventRepo.Find(specification);
        }
        
        public async Task<IEnumerable<EventParticipant>> GetEventParticipantsAsync(int eventId, bool? isApproved = null)
        {
            // Use specification pattern for participant query
            var specification = new EventParticipantSpecification()
                .ByEventId(eventId);
            
            if (isApproved.HasValue)
                specification.ByApprovalStatus(isApproved.Value);
                
            return await _participantRepo.Find(specification);
        }
        
        public async Task<bool> ApproveParticipantAsync(int eventId, int participantId)
        {
            var participant = await _participantRepo.FindFirstByExpressionAsync(
                p => p.Id == participantId && p.EventId == eventId
            );
            
            if (participant == null)
                return false;
                
            participant.IsApproved = true;
            _participantRepo.Update(participant);
            await _participantRepo.SaveAsync();
            return true;
        }
        
        public async Task<bool> RejectParticipantAsync(int eventId, int participantId)
        {
            var participant = await _participantRepo.FindFirstByExpressionAsync(
                p => p.Id == participantId && p.EventId == eventId
            );
            
            if (participant == null)
                return false;
                
            _participantRepo.Delete(participant);
            await _participantRepo.SaveAsync();
            return true;
        }
        
        public async Task<bool> RemoveParticipantAsync(int eventId, int participantId)
        {
            var participant = await _participantRepo.FindFirstByExpressionAsync(
                p => p.Id == participantId && p.EventId == eventId
            );
            
            if (participant == null)
                return false;
                
            _participantRepo.Delete(participant);
            await _participantRepo.SaveAsync();
            return true;
        }
        
        public async Task<bool> CancelEventAsync(int eventId)
        {
            var ev = await _eventRepo.GetByIdAsync(eventId);
            if (ev == null)
                return false;
                
            // Mark as cancelled instead of deleting
            ev.IsCancelled = true;
            _eventRepo.Update(ev);
            await _eventRepo.SaveAsync();
            return true;
        }
    }
}
