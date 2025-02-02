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
    }
}