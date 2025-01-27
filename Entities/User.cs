
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
            modelBuilder.Entity<Route>().Property(r => r.StartPoint).HasColumnType("geography");
            modelBuilder.Entity<Route>().Property(r => r.EndPoint).HasColumnType("geography");
            modelBuilder.Entity<RoutePoint>().Property(r => r.Point).HasColumnType("geography");
//             modelBuilder.Entity<RouteTag>()
//                  .HasKey(rt => new { rt.RouteId, rt.TagId });
//             modelBuilder.Entity<RoutesTypes>()
//  .HasKey(rt => new { rt.RouteId, rt.RouteTypeId });

// modelBuilder.Entity<RouteTag>()
//     .HasOne(rt => rt.Route)
//     .WithMany(r => r.RouteTags)
//     .HasForeignKey(rt => rt.RouteId);

// modelBuilder.Entity<RouteTag>()
//     .HasOne(rt => rt.Tag)
//     .WithMany(t => t.RouteTags)
//     .HasForeignKey(rt => rt.TagId);

modelBuilder.Entity<EventActivity>()
    .HasKey(ea => new { ea.EventId, ea.ActivityTypeId });

modelBuilder.Entity<EventActivity>()
    .HasOne(ea => ea.Event)
    .WithMany(e => e.EventActivities)
    .HasForeignKey(ea => ea.EventId);

modelBuilder.Entity<EventActivity>()
    .HasOne(ea => ea.ActivityType)
    .WithMany(at => at.EventActivities)
    .HasForeignKey(ea => ea.ActivityTypeId);
    modelBuilder.Entity<GroupActivity>()
    .HasKey(ga => new { ga.GroupId, ga.ActivityTypeId });

modelBuilder.Entity<GroupActivity>()
    .HasOne(ga => ga.Group)
    .WithMany(g => g.GroupActivities)
    .HasForeignKey(ga => ga.GroupId);

modelBuilder.Entity<GroupActivity>()
    .HasOne(ga => ga.ActivityType)
    .WithMany(at => at.GroupActivities)
    .HasForeignKey(ga => ga.ActivityTypeId);

        }
        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<RouteType> RouteType { get; set; }
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
