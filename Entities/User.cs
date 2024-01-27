
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
        public string ?LastName { get; set; }
        public string ?PhoneNumber { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? EditOn { get; set; }
        public string ?ProfilePictureUrl { get; set; }
        public string ?Bio { get; set; }
        public string ?Address { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiration { get; set; }
        public bool? isVerified { get; set; }
        // language
        public int countryId { get; set; }

    }
    public class RegistrateModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    // DbContext class for the database
    public class MotoMeetDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteTag>()
                 .HasKey(rt => new { rt.RouteId, rt.TagId });
                   modelBuilder.Entity<RoutesTypes>()
        .HasKey(rt => new { rt.RouteId, rt.RouteTypeId });
        }
        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<RouteTag> RouteTag { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<RouteType> RouteType { get; set; }
        public DbSet<RoutesTypes> RoutesTypes { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();


            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConfigDB"), x => x.UseNetTopologySuite());

        }

    }




}
