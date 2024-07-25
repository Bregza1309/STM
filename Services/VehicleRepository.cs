using Microsoft.EntityFrameworkCore;
using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;
namespace STM.Services
{
    public class VehicleRepository
    {
        StmContext Context { get; set; }
        public VehicleRepository(StmContext context) {  Context = context; }
        public void Add(Vehicle vehicle)
        {
            Context.Vehicles.Add(vehicle);
            Context.SaveChanges();
        }
        public Vehicle Get(int Id)
        {
            return Context.Vehicles.Include(v => v.Driver).SingleOrDefault(v => v.Id == Id)!;
        } 
        public void Update(Vehicle vehicle)
        {
            Context.ChangeTracker.Clear();
            Context.Vehicles.Update(vehicle);
            Context.SaveChanges();
        }
        public bool IsRegistered(string registrationNumber)
        {
            return Context.Vehicles.Where(v => v.RegistrationNumber == registrationNumber).Any();
        }
        public void Delete(int Id) {
            Vehicle existing = Get(Id);
            if (existing != null)
            {
                Context.Vehicles.Remove(existing);
                Context.SaveChanges();
            }
        }
    }
}
