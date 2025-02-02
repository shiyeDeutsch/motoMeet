using Microsoft.AspNetCore.SignalR;
namespace motoMeet {public class ChatMessage
{
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? StageId { get; set; }
}
public class LocationUpdate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Altitude { get; set; }
    public double Speed { get; set; } // in m/s, km/h, etc. (your choice)
    public int? StageId { get; set; } // if null, broadcast to entire event group
}


public class EventHub : Hub
{
    // When the client connects to this hub, we can read the `eventId` from the query string.
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            await base.OnConnectedAsync();
            return;
        }

        // Try reading the 'eventId' from query param: e.g. ws://.../EventHub?eventId=123
        var eventIdString = httpContext.Request.Query["eventId"].ToString();
        if (!string.IsNullOrWhiteSpace(eventIdString) && int.TryParse(eventIdString, out int eventId))
        {
            // Add this connection to the main event group "event_{eventId}"
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventId}");
        }
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Let the client explicitly join a stage group inside the event if they want stage-specific updates.
    /// For example, "event_1001_stage_501".
    /// </summary>
    public async Task JoinStage(int eventId, int stageId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventId}_stage_{stageId}");
        // Optionally inform the user or others
        await Clients.Caller.SendAsync("StageJoined", new { eventId, stageId });
    }

    /// <summary>
    /// Let the client explicitly leave a stage group if they no longer want that stage's updates.
    /// </summary>
    public async Task LeaveStage(int eventId, int stageId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event_{eventId}_stage_{stageId}");
        await Clients.Caller.SendAsync("StageLeft", new { eventId, stageId });
    }

    /// <summary>
    /// Receive a location update from a user. 
    /// If StageId is provided, broadcast to that stage group only; otherwise to entire event.
    /// </summary>
    public async Task SendLocationUpdate(int eventId, LocationUpdate location)
    {
        // Build a broadcast payload
        var payload = new
        {
            ConnectionId = Context.ConnectionId,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Altitude = location.Altitude,
            Speed = location.Speed,
            StageId = location.StageId,
            Timestamp = DateTime.UtcNow
        };

        if (location.StageId.HasValue)
        {
            // Broadcast to the stage group only
            var stageGroup = $"event_{eventId}_stage_{location.StageId.Value}";
            await Clients.Group(stageGroup).SendAsync("ReceiveLocationUpdate", payload);
        }
        else
        {
            // Broadcast to entire event group
            var eventGroup = $"event_{eventId}";
            await Clients.Group(eventGroup).SendAsync("ReceiveLocationUpdate", payload);
        }
    }

    /// <summary>
    /// Send a chat message to either a specific stage or entire event.
    /// If ChatMessage.StageId is null, it goes to entire event group.
    /// </summary>
    public async Task SendChatMessage(int eventId, ChatMessage message)
    {
        // Ensure there's a valid message
        if (string.IsNullOrWhiteSpace(message.Text)) return;

        message.CreatedAt = DateTime.UtcNow;

        if (message.StageId.HasValue)
        {
            // broadcast to stage group
            var stageGroup = $"event_{eventId}_stage_{message.StageId.Value}";
            await Clients.Group(stageGroup).SendAsync("ReceiveChatMessage", message);
        }
        else
        {
            // broadcast to entire event group
            var eventGroup = $"event_{eventId}";
            await Clients.Group(eventGroup).SendAsync("ReceiveChatMessage", message);
        }
    }
}
}