using motoMeet;

namespace motoMeet
{
public enum NotificationType
{
    // Basic user interactions
    Follow,
    Unfollow,

    // Post/Comment interactions
    Like,
    Comment,
    Mention,
    // etc.

    // Group / Event interactions
    JoinedEvent,
    JoinedGroup,
    // Or you can keep them in a single "Joined" with the entity type in `TargetEntityType`

    // You can add more as needed
}

public class Notification : EntityBase
{
    public int Id { get; set; }

    // Who receives this notification
    public int RecipientId { get; set; }
    public virtual Person Recipient { get; set; }

    // Who triggered it (e.g. the person who liked/followed, etc.)
    public int ActorId { get; set; }
    public virtual Person Actor { get; set; }

    // The type or category of notification (like, comment, follow, etc.)
    public NotificationType NotificationType { get; set; }

    // Optional short message text or template
    public string? Message { get; set; }

    // Whether the user has seen or opened it
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ReadOn { get; set; }

    // Polymorphic reference: which entity this notification is about?
    // e.g. a group post, an event, a route, etc.
    public string? TargetEntityType { get; set; }
    public int? TargetEntityId { get; set; }
}

public class Reaction : EntityBase
{
    public int Id { get; set; }

    // Which user performed the reaction
    public int PersonId { get; set; }
    public virtual Person Person { get; set; }

    // The type of reaction: Like, Love, Angry, etc.
    public ReactionType ReactionType { get; set; }

    public DateTime CreatedOn { get; set; }

    // Polymorphic reference to the target entity
    public string TargetEntityType { get; set; }  // e.g. "GroupPost", "Review", "EventPost"
    public int TargetEntityId { get; set; }
}
public enum ReactionType
{
    Like,
    Love,
    Haha,
    Wow,
    Sad,
    Angry,
    // Or keep it very simple with just "Like"
}
public class Favorite : EntityBase
{
    public int Id { get; set; }

    public int PersonId { get; set; }
    public virtual Person Person { get; set; }

    // Polymorphic references again
    public string TargetEntityType { get; set; }
    public int TargetEntityId { get; set; }

    public DateTime CreatedOn { get; set; }
}


}
