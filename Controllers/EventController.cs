using Microsoft.AspNetCore.Mvc;

namespace motoMeet
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventManager _eventManager;

        public EventsController(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        // 1) Create an Event (with or without initial stages)
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] CreateEventRequest request)
        {
            try
            {
                var ev = await _eventManager.CreateEventAsync(request);
                return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ev);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2) Fetch an event
        [HttpGet("{eventId}")]
        public async Task<ActionResult<Event>> GetEvent(int eventId)
        {
            try
            {
                var ev = await _eventManager.GetEventAsync(eventId);
                if (ev == null) return NotFound();
                return Ok(ev);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3) Add a stage to an existing event
        [HttpPost("{eventId}/stages")]
        public async Task<ActionResult<EventStage>> AddStage(int eventId, [FromBody] CreateEventStageRequest request)
        {
            try
            {
                var stage = await _eventManager.AddStageAsync(eventId, request);
                return Ok(stage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 4) Join an event
        [HttpPost("{eventId}/join")]
        public async Task<ActionResult<EventParticipant>> JoinEvent(int eventId, [FromBody] JoinEventRequest request)
        {
            try
            {
                var participant = await _eventManager.JoinEventAsync(eventId, request);
                return Ok(participant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 5) Join a stage (start traveling route)
        [HttpPost("{eventId}/stages/{stageId}/join")]
        public async Task<ActionResult<EventStageParticipant>> JoinStage(int eventId, int stageId, [FromBody] JoinStageRequest request)
        {
            try
            {
                var stageParticipant = await _eventManager.JoinStageAsync(eventId, stageId, request);
                return Ok(stageParticipant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // 6) Get events with filtering
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents([FromQuery] EventFilterRequest request)
        {
            try
            {
                var events = await _eventManager.GetEventsAsync(request);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 7) Update an event
        [HttpPut("{eventId}")]
        public async Task<ActionResult> UpdateEvent(int eventId, [FromBody] UpdateEventRequest request)
        {
            try
            {
                var success = await _eventManager.UpdateEventAsync(eventId, request);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 8) Delete an event
        [HttpDelete("{eventId}")]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            try
            {
                var success = await _eventManager.DeleteEventAsync(eventId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 9) Cancel an event
        [HttpPost("{eventId}/cancel")]
        public async Task<ActionResult> CancelEvent(int eventId)
        {
            try
            {
                var success = await _eventManager.CancelEventAsync(eventId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 10) Get event participants
        [HttpGet("{eventId}/participants")]
        public async Task<ActionResult<IEnumerable<Person>>> GetEventParticipants(int eventId)
        {
            try
            {
                var participants = await _eventManager.GetEventParticipantsAsync(eventId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 11) Get pending event participants
        [HttpGet("{eventId}/pending-participants")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPendingParticipants(int eventId)
        {
            try
            {
                var pendingParticipants = await _eventManager.GetPendingParticipantsAsync(eventId);
                return Ok(pendingParticipants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 12) Approve participant
        [HttpPost("{eventId}/participants/{participantId}/approve")]
        public async Task<ActionResult> ApproveParticipant(int eventId, int participantId)
        {
            try
            {
                var success = await _eventManager.ApproveParticipantAsync(eventId, participantId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 13) Reject participant
        [HttpPost("{eventId}/participants/{participantId}/reject")]
        public async Task<ActionResult> RejectParticipant(int eventId, int participantId)
        {
            try
            {
                var success = await _eventManager.RejectParticipantAsync(eventId, participantId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 14) Remove participant
        [HttpDelete("{eventId}/participants/{participantId}")]
        public async Task<ActionResult> RemoveParticipant(int eventId, int participantId)
        {
            try
            {
                var success = await _eventManager.RemoveParticipantAsync(eventId, participantId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 15) Leave event
        [HttpPost("{eventId}/leave")]
        public async Task<ActionResult> LeaveEvent(int eventId, [FromBody] JoinEventRequest request)
        {
            try
            {
                var success = await _eventManager.LeaveEventAsync(eventId, request.PersonId);
                if (success)
                    return NoContent();
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 16) Check if user is event creator
        [HttpGet("{eventId}/is-creator/{personId}")]
        public async Task<ActionResult<bool>> IsEventCreator(int eventId, int personId)
        {
            try
            {
                var isCreator = await _eventManager.IsEventCreatorAsync(eventId, personId);
                return Ok(isCreator);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 17) Check if user is event participant
        [HttpGet("{eventId}/is-participant/{personId}")]
        public async Task<ActionResult<bool>> IsEventParticipant(int eventId, int personId)
        {
            try
            {
                var isParticipant = await _eventManager.IsEventParticipantAsync(eventId, personId);
                return Ok(isParticipant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 18) Get event creator name
        [HttpGet("{eventId}/creator-name")]
        public async Task<ActionResult<string>> GetEventCreatorName(int eventId)
        {
            try
            {
                var creatorName = await _eventManager.GetEventCreatorNameAsync(eventId);
                return Ok(creatorName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 19) Get event participant count
        [HttpGet("{eventId}/participant-count")]
        public async Task<ActionResult<int>> GetEventParticipantCount(int eventId)
        {
            try
            {
                var count = await _eventManager.GetEventParticipantCountAsync(eventId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}