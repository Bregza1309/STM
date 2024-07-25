using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using StudentTransportManagement_STM_.Server.Context;
using Microsoft.EntityFrameworkCore;

namespace StudentTransportManagement_STM_.Server.SeedData
{
    public static class DataSeeding
    {
        public static void EnsureDataSeeding(IApplicationBuilder app)
        {
            StmContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StmContext>();
            UserManager<ApplicationUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //check for any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            //get all users registered to the system
            ApplicationUser[] applicationUsers = userManager.Users.ToArray().Where(p => userManager.IsInRoleAsync(p, "Admin").Result.Equals(false)).OrderBy(p => p.UserName).ToArray();

            //add a default parent
            if (!context.Parents.Any())
            {
                context.Parents.Add(new Parent() {  UserId = applicationUsers[1].Id });
                context.SaveChanges();
            }
            //add a default driver
            if(!context.Drivers.Any())
            {
                context.Drivers.Add(new Driver() {  UserId = applicationUsers[0].Id , Verified = true });
            }
            //add a default student
            if (!context.Students.Any())
            {
                context.Students.Add(new Student() {Firstname = "Gary", Lastname = "Wright",  DriverId = 1, ParentId = 1 ,Gender = "MALE" ,Grade = "6"});
                context.SaveChanges();
            }
            //add a default latestMessage
            if(!context.News.Any())
            {
                context.News.Add(new LatestNews { Title = "Welcome to STM ", Description = "Coming soon !!!!!!!!! 31st October 2023" });
                context.SaveChanges();
            }
            //add a default route
            if (!context.Routes.Any())
            {
                context.Routes.Add(new Shared.DataModels.Route { RouteName = "Route to Jeppe College" ,StartingPoint = "Meyerdale" , EndingPoint = "Jeppe College"});
                context.SaveChanges();
            }
            //add a default message 
            if (!context.Messages.Any())
            {
                context.Messages.Add(new Message() { Content = "You are late my guy" , Sender_Id = applicationUsers[0].Id , Recipient_Id = applicationUsers[1].Id , IsRead = false });
                context.SaveChanges();
            }
            //add a default trip
            if (!context.Trips.Any())
            {
                context.Trips.Add(new Trip() { RouteId = 1 , Departure = DateTime.Now , Arrival = DateTime.Now , StudentId = 1});
                context.SaveChanges();
            }
            //add a default vehicle 
            if (!context.Vehicles.Any())
            {
                context.Vehicles.Add(new Vehicle { Description = "White Toyota Quantum", RegistrationNumber = "CN67HJ", StudentCapacity = 15, DriverId = 1 }) ;
            }
            //add some schools to the database
            if (!context.Schools.Any())
            {
                string[] names = { "GLENRIDGE PRIMARY", "CURRO ACCADEMY", "Randvaal Primary" };
                var schools = names.Select(n => new School()
                {
                    Name = n,
                    Drivers = new List<Driver>()
                    {
                        context.Drivers.First()
                    }
                }).ToList();
                schools[0].Students.Add(context.Students.First());
                context.Schools.AddRange(schools);
                context.SaveChanges();
            }

        }
    }
}
