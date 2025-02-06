using System;
using System.Threading.Tasks;

namespace motoMeet
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(Event ev);
        Task<EventStage> CreateEventStageAsync(EventStage stage);
        Task<EventParticipant> CreateEventParticipantAsync(EventParticipant participant);
        Task<EventStageParticipant> CreateEventStageParticipantAsync(EventStageParticipant stageParticipant);

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
    }

    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IRepository<EventStage> _stageRepo;
        private readonly IRepository<EventParticipant> _participantRepo;
        private readonly IRepository<EventStageParticipant> _stageParticipantRepo;

        public EventService(
            IRepository<Event> eventRepo,
            IRepository<EventStage> stageRepo,
            IRepository<EventParticipant> participantRepo,
            IRepository<EventStageParticipant> stageParticipantRepo)
        {
            _eventRepo = eventRepo;
            _stageRepo = stageRepo;
            _participantRepo = participantRepo;
            _stageParticipantRepo = stageParticipantRepo;
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
    }
}
