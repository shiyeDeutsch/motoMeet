using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace motoMeet
{


    public class Person : EntityBase
    {
        public int Id { get; set; }  // PK (override if you want from EntityBase)

        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? EditOn { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiration { get; set; }
        public bool? IsVerified { get; set; }
        public int CountryId { get; set; }
        public double TotalDistance { get; set; }

        // A Person can have many UserRoute records
        public virtual ICollection<UserRoute> UserRoutes { get; set; } = new List<UserRoute>();
        public virtual ICollection<PersonFollow> Followers { get; set; } = new List<PersonFollow>();
        public virtual ICollection<PersonFollow> Following { get; set; } = new List<PersonFollow>();

        // Example: If a Person can be in multiple groups, you'll handle that via GroupMember.
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();

        // If we store the routes created by the user:
        public virtual ICollection<Route> CreatedRoutes { get; set; } = new List<Route>();

        // If we store events created by the user:
        public virtual ICollection<Event> CreatedEvents { get; set; } = new List<Event>();


        public virtual ICollection<Notification> NotificationsReceived { get; set; }
            = new List<Notification>();

        // The user’s reactions (if you want a back-nav from Reaction to Person)
        public virtual ICollection<Reaction> Reactions { get; set; }
            = new List<Reaction>();

        // Favorites as well
        public virtual ICollection<Favorite> Favorites { get; set; }
            = new List<Favorite>();

        public virtual ICollection<PointOfInterest> PointsOfInterest { get; set; }
            = new List<PointOfInterest>();
    }


    public class UserDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public int? CountryId { get; set; }
        public string? Token { get; set; }
        public UserDto() { }
        public UserDto(Person person)
        {
            Id = person.Id;
            Username = person.Username;
            FirstName = person.FirstName;
            LastName = person.LastName;
            Email = person.Email;
            PhoneNumber = person.PhoneNumber;
            Bio = person.Bio;
            Address = person.Address;
            CountryId = person.CountryId;

        }
    }

    public class RegistrateModel
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int CountryId { get; set; }
    }


    // DbContext class for the database
    public class MotoMeetDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------- Geographic types ----------------
            modelBuilder.Entity<Route>()
                .Property(r => r.StartPoint)
                .HasColumnType("geography");
            modelBuilder.Entity<Route>()
                .Property(r => r.EndPoint)
                .HasColumnType("geography");

            modelBuilder.Entity<RoutePoint>()
                .Property(rp => rp.Point)
                .HasColumnType("geography");

            modelBuilder.Entity<UserRoutePoint>()
                .Property(urp => urp.Point)
                .HasColumnType("geography");

            modelBuilder.Entity<PointOfInterest>()
                .Property(poi => poi.Location)
                .HasColumnType("geography");

            // ---------------- Enum Conversions ----------------
            modelBuilder.Entity<EventStage>()
                .Property(es => es.StageType)
                .HasConversion<string>();

            modelBuilder.Entity<PointOfInterest>()
                .Property(poi => poi.WaypointType)
                .HasConversion<string>();

            // ---------------- Composite Keys & Relationships ----------------
            // EventActivity: many-to-many between Event and ActivityType
            modelBuilder.Entity<EventActivity>()
                .HasKey(ea => new { ea.EventId, ea.ActivityTypeId });
            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventActivities)
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.ActivityType)
                .WithMany(at => at.EventActivities)
                .HasForeignKey(ea => ea.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // GroupActivity: many-to-many between Group and ActivityType
            modelBuilder.Entity<GroupActivity>()
                .HasKey(ga => new { ga.GroupId, ga.ActivityTypeId });
            modelBuilder.Entity<GroupActivity>()
                .HasOne(ga => ga.Group)
                .WithMany(g => g.GroupActivities)
                .HasForeignKey(ga => ga.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GroupActivity>()
                .HasOne(ga => ga.ActivityType)
                .WithMany(at => at.GroupActivities)
                .HasForeignKey(ga => ga.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Person & PersonFollow ----------------
            // PersonFollow primary key and relationships
            modelBuilder.Entity<PersonFollow>()
                .HasKey(pf => pf.Id);
            modelBuilder.Entity<PersonFollow>()
                .HasOne(pf => pf.Follower)
                .WithMany(p => p.Following) // Person.Following: persons that this user is following
                .HasForeignKey(pf => pf.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PersonFollow>()
                .HasOne(pf => pf.Following)
                .WithMany(p => p.Followers) // Person.Followers: persons following this user
                .HasForeignKey(pf => pf.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Notification, Reaction, Favorite ----------------
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Recipient)
                .WithMany(p => p.NotificationsReceived) // Optionally add a collection navigation on Person
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Actor)
                .WithMany() // Optionally add a collection navigation on Person
                .HasForeignKey(n => n.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Person)
                .WithMany(p => p.Reactions) // Optionally add a collection navigation on Person
                .HasForeignKey(r => r.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Person)
                .WithMany(p => p.Favorites) // Optionally add a collection navigation on Person
                .HasForeignKey(f => f.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PointOfInterest>()
                .HasOne(poi => poi.Person)
                .WithMany(p => p.PointsOfInterest)
                .HasForeignKey(poi => poi.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Route, Reviews, and UserRoute ----------------
            // Route: One-to-many with Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Route)
                .WithMany(rt => rt.Reviews)
                .HasForeignKey(r => r.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserRoute: relationships with Person and Route
            modelBuilder.Entity<UserRoute>()
                .HasOne(ur => ur.Person)
                .WithMany(p => p.UserRoutes)  // Optionally add a navigation property on Person
                .HasForeignKey(ur => ur.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRoute>()
                .HasOne(ur => ur.Route)
                .WithMany(r => r.UserRoutes)
                .HasForeignKey(ur => ur.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Event and Its Child Entities ----------------
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(p => p.CreatedEvents) // Optionally add a collection navigation on Person (e.g. CreatedEvents)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Events)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EventStage>()
                .HasOne(es => es.Event)
                .WithMany(e => e.Stages)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventStage>()
                .HasOne(es => es.Route)
                .WithMany() // Route may not need a navigation back here
                .HasForeignKey(es => es.RouteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.Person)
                .WithMany() // Optionally add a navigation property on Person
                .HasForeignKey(ep => ep.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventStageParticipant>()
                .HasOne(esp => esp.EventStage)
                .WithMany(es => es.StageParticipants)
                .HasForeignKey(esp => esp.EventStageId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventStageParticipant>()
                .HasOne(esp => esp.EventParticipant)
                .WithMany(ep => ep.StageParticipants)
                .HasForeignKey(esp => esp.EventParticipantId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventStageParticipant>()
                .HasOne(esp => esp.UserRoute)
                .WithOne(ur => ur.EventStageParticipant) // No navigation property assumed on UserRoute
           .HasForeignKey<UserRoute>(ur => ur.EventStageParticipantId).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EventItem>()
                .HasOne(ei => ei.Event)
                .WithMany(e => e.RequiredItems)
                .HasForeignKey(ei => ei.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Group and Its Related Entities ----------------
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany() // Optionally add a navigation property on Person
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Person)
                .WithMany(p => p.GroupMemberships) // Optionally add a navigation property on Person
                .HasForeignKey(gm => gm.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupPost>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(gp => gp.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GroupPost>()
                .HasOne(gp => gp.Author)
                .WithMany() // Optionally add a navigation property on Person (e.g. Posts)
                .HasForeignKey(gp => gp.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupPostComment>()
                .HasOne(gpc => gpc.GroupPost)
                .WithMany(gp => gp.Comments)
                .HasForeignKey(gpc => gpc.GroupPostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GroupPostComment>()
                .HasOne(gpc => gpc.Author)
                .WithMany() // Optionally add a navigation property on Person
                .HasForeignKey(gpc => gpc.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupPostAttachment>()
                .HasOne(gpa => gpa.GroupPost)
                .WithMany(gp => gp.Attachments)
                .HasForeignKey(gpa => gpa.GroupPostId)
                .OnDelete(DeleteBehavior.Cascade);


        }
        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PersonFollow> PersonFollows { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventStage> EventStages { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<EventStageParticipant> EventStageParticipants { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<EventActivity> EventActivities { get; set; }
        public DbSet<GroupActivity> GroupActivities { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<EventItem> EventItems { get; set; }
        public DbSet<RouteType> RouteTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<UserRoute> UserRoutes { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        public DbSet<UserRoutePoint> UserRoutePoints { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConfigDB"), (x) => { x.UseNetTopologySuite(); });

        }

    }




}
