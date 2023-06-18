using System;
using Microsoft.EntityFrameworkCore;

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
    }

    // DbContext class for the database
    public class MotoMeetDbContext : DbContext
    {

        public MotoMeetDbContext(DbContextOptions<MotoMeetDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConfigDB"));

        }



        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     var configuration = new ConfigurationBuilder()
        //         .SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile("appsettings.json")
        //         .Build();

        //     var connectionString = configuration.GetConnectionString("ConfigDB");
        //     optionsBuilder.UseSqlServer(connectionString);
        // }
    }





}
