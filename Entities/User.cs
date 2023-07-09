 
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace motoMeet
{
    // Class representing the Persons table
  public class Person
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RoleID { get; set; }
    public string NationalCode { get; set; }
    public DateTime? AddedOn { get; set; }
    public DateTime? EditOn { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
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
