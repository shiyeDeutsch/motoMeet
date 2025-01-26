
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace motoMeet
{
    // Class representing the Persons table
    public class Person : EntityBase
    {
        public string? Username { get; set; }
        public int Id { get; set; }
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
        // language
        public int CountryId { get; set; }
        public double TotalDistance { get; set; }
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
        }
        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
      //  public DbSet<RouteTag> RouteTags { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<RouteType> RouteType { get; set; }
        // public DbSet<RoutesTypes> RoutesTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        // public DbSet<DifficultyLevel> DifficultyLevels { get; set; }

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
