using Microsoft.EntityFrameworkCore;
using StudentTransportManagement_STM_.Shared.DataModels;
namespace StudentTransportManagement_STM_.Server.Context
{
    public class StmContext:DbContext
    {
        public StmContext(DbContextOptions<StmContext> opt) : base(opt) { }
        public DbSet<Student> Students { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LatestNews> News { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Shared.DataModels.Route> Routes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Verification> Verifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                 .Navigation(e => e.School)
                 .AutoInclude();
            modelBuilder.Entity<Student>()
                 .Navigation(e => e.Driver)
                 .AutoInclude();
            modelBuilder.Entity<Student>()
                 .Navigation(e => e.Parent)
                 .AutoInclude();
            
           
            modelBuilder.Entity<Driver>()
                .Navigation(e => e.Students)
                .AutoInclude();
            modelBuilder.Entity<Driver>()
                .Navigation(e => e.Vehicles)
                .AutoInclude();

            modelBuilder.Entity<Request>()
                .Navigation(e => e.Student)
                .AutoInclude();
            modelBuilder.Entity<Request>()
                .Navigation(e => e.Parent)
                .AutoInclude();
            modelBuilder.Entity<Request>()
                .Navigation(e => e.Driver)
                .AutoInclude();
            //Auto include driver and vehicle naavigations for the verification entity
            modelBuilder.Entity<Verification>()
                .Navigation(e => e.Driver)
                .AutoInclude ();
            modelBuilder.Entity<Verification>()
                .Navigation(e => e.Vehicle)
                .AutoInclude();


        }

    }
}
