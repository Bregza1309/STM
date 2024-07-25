using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace STM.Services
{
    public class DriverService : IService<Driver>
    {
        StmContext context;
        UserManager<ApplicationUser> userManager;
        public DriverService(StmContext context, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public void Create(Driver driver)
        {
            Driver? existing = context.Drivers.SingleOrDefault(d => d.UserId == driver.UserId);
            if (existing == null)
            {
                context.Drivers.Add(driver);
                context.SaveChanges();
            }
        }
        public Dictionary<ApplicationUser, Driver> Search(string key)
        {
            var drivers = Get();
            Regex rx = new Regex(@$"{key.ToLower()}");
            foreach(var kvp in drivers)
            {
                var appUser = kvp.Key;
                if(!rx.IsMatch(appUser.Firstname.ToLower()) && !rx.IsMatch(appUser.Firstname.ToLower())
                    && !rx.IsMatch(appUser.UserName.ToLower()))
                {
                    drivers.Remove(kvp.Key);
                }
            }
            return drivers;

        }
        public Dictionary<ApplicationUser, Driver> Get(string? userId = null)
        {
            Dictionary<ApplicationUser,Driver> keyValuePairs = new Dictionary<ApplicationUser, Driver>();
            if(string.IsNullOrEmpty(userId))
            {
                List<Driver> drivers =  context.Drivers
                    .Include(d => d.Students)
                    .Include(d => d.Requests)
                    .Include(d => d.Vehicles)
                    .OrderBy(d => d.UserId)
                    .ToList();
                List<ApplicationUser> users = userManager.GetUsersInRoleAsync("driver").Result.OrderBy(d => d.Id).ToList();
                for(int i = 0; i < drivers.Count; i++)
                {
                    keyValuePairs.Add(users[i], drivers[i]);
                }
            }
            else
            {
                Driver driver = context.Drivers
                    .Include(d => d.Students)
                    .Include(d => d.Requests)
                    .Include(d => d.Vehicles)
                    .SingleOrDefault(d => d.UserId == userId) ?? new Driver();
                ApplicationUser user = userManager.FindByIdAsync(userId).Result;
                keyValuePairs.Add(user, driver);
            }
            return keyValuePairs;
        }
        public void Update(Driver driver)
        {
            context.Drivers.Update(driver);
            context.SaveChanges();
        }
        public List<Student> SearchStudents(string key, string driverId)
        {
            try
            {
                Regex rx = new ($@"{key.ToLower()}");
                var students = Get(driverId).Values.First().Students;
                return students.Where(s => rx.IsMatch(s.Firstname.ToLower()) || rx.IsMatch(s.Lastname.ToLower()) || rx.IsMatch(s.Gender.ToLower()) || rx.IsMatch(s.School!.Name.ToLower())).ToList();
            }
            catch(RegexParseException)
            {
                return new List<Student>();
            }
        }

        public void Delete(string Id)
        {
            var Driver = Get(Id).First();

            //Remove the vehicles associated with the driver
            context.Vehicles.RemoveRange(Driver.Value.Vehicles);
            //Remove Requests associated with the driver
            context.Requests.RemoveRange(Driver.Value.Requests);
            //Delete the driver entity
            context.Drivers.Remove(Driver.Value);
            userManager.DeleteAsync(Driver.Key).Wait();
            context.SaveChanges();
        }
    }
}
