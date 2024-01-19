
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace motoMeet
{
    // Class representing the Persons table
    public class Person
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? EditOn { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        //language
        // country
        //isVerified
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
            //     modelBuilder.Entity<Route>()
            //         .Property(e => e.StartPoint)
            //         .HasConversion(point => point.AsText(), location => (Point)new WKTReader().Read(location));
            //  modelBuilder.Entity<Route>()
            //         .Property(e => e.EndPoint)
            //         .HasConversion(point => point.AsText(), location => (Point)new WKTReader().Read(location));

            //     modelBuilder.Entity<RoutePoint>()
            //         .Property(e => e.Point)
            //         .HasConversion(point => point.AsText(), location => (Point)new WKTReader().Read(location));
            //     base.OnModelCreating(modelBuilder);
        }

        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
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
