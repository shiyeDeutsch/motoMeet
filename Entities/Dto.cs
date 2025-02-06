    namespace motoMeet
{
    namespace motoMeet.DTOs
{
    public class UserDto
    {
        // Basic user details
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? AddedOn { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public double TotalDistance { get; set; }
        
        // For authentication
        public string? Token { get; set; }
        
        // Nested collections
        public List<RouteDto> UserRoutes { get; set; }
        public List<EventDto> ParticipatedEvents { get; set; }  // (if you later load events the user is taking part in)
        public List<GroupDto> Groups { get; set; }
        public List<RouteDto> CreatedRoutes { get; set; }
        public List<EventDto> CreatedEvents { get; set; }

        public UserDto()
        {
            UserRoutes = new List<RouteDto>();
            ParticipatedEvents = new List<EventDto>();
            Groups = new List<GroupDto>();
            CreatedRoutes = new List<RouteDto>();
            CreatedEvents = new List<EventDto>();
        }

        // Mapping constructor: supply a Person entity and map the details.
        public UserDto(Person person) : this()
        {
            Id = person.Id;
            Email = person.Email;
            Username = person.Username;
            FirstName = person.FirstName;
            LastName = person.LastName;
            PhoneNumber = person.PhoneNumber;
            AddedOn = person.AddedOn;
            ProfilePictureUrl = person.ProfilePictureUrl;
            Bio = person.Bio;
            Address = person.Address;
            TotalDistance = person.TotalDistance;

            // Map the routes the user has logged via the join table.
            if (person.UserRoutes != null)
            {
                UserRoutes = person.UserRoutes
                    .Select(ur => new RouteDto(ur.Route))
                    .ToList();
            }
            
            // Map groups from the memberships.
            if (person.GroupMemberships != null)
            {
                Groups = person.GroupMemberships
                    .Select(gm => new GroupDto(gm.Group))
                    .ToList();
            }
            
            // Map the routes created by the user.
            if (person.CreatedRoutes != null)
            {
                CreatedRoutes = person.CreatedRoutes
                    .Select(cr => new RouteDto(cr))
                    .ToList();
            }
            
            // Map the events created by the user.
            if (person.CreatedEvents != null)
            {
                CreatedEvents = person.CreatedEvents
                    .Select(ce => new EventDto(ce))
                    .ToList();
            }
            
            // Note: Person does not have a direct collection of "participated events"
            // so you may want to load that from your EventManager and then set ParticipatedEvents later.
        }
    }

    public class RouteDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Length { get; set; }
        public double Rating { get; set; }
        public bool IsLoop { get; set; }
        // You can add additional properties as needed

        public RouteDto() { }

        // Map from the Route entity
        public RouteDto(Route route)
        {
            if (route == null) return;
            Id = route.Id;
            Name = route.Name;
            Description = route.Description;
            Length = route.Length;
            Rating = route.Rating;
            IsLoop = route.isLoop;
        }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsPublic { get; set; }
        // Add additional fields as needed

        public EventDto() { }

        public EventDto(Event evt)
        {
            if (evt == null) return;
            Id = evt.Id;
            Name = evt.Name;
            Description = evt.Description;
            StartDateTime = evt.StartDateTime;
            EndDateTime = evt.EndDateTime;
            IsPublic = evt.IsPublic;
        }
    }

    public class GroupDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        // Add any other properties you’d like to expose

        public GroupDto() { }

        public GroupDto(Group group)
        {
            if (group == null) return;
            Id = group.Id;
            Name = group.Name;
            Description = group.Description;
        }
    }
}

}