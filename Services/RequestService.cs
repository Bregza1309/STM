using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;
using Microsoft.EntityFrameworkCore;
namespace STM.Services
{
    public class RequestService
    {
        StmContext Context { get; set; }
        public RequestService(StmContext context)
        {
            Context = context;
        }
        public void Request(Request request)
        {
            Context.Requests.Add(request);
            Context.SaveChanges();
        }
        public void ProcessRequest(Request request)
        {
            Student learner = request.Student!;
            if(learner.Driver == null)
            {
                if (request.Accepted.HasValue)
                {
                    if (request.Accepted.Value)
                    {
                        learner.DriverId = request.DriverId;

                        Context.Students.Update(learner);
                    }
                    Context.ChangeTracker.Clear();
                    Context.Requests.Update(request);
                    Context.SaveChanges();
                }
            }
        }
        public List<Request> GetAll() => Context.Requests
            .Include(r => r.Parent)
            .Include(r => r.Student)
            .Include(r => r.Driver)
            .ToList();
        public Request Get(int Id)
        {
            return Context.Requests
                .Include(r => r.Parent)
                .Include(r => r.Student)
                .Include(r => r.Driver)
                .SingleOrDefault(r => r.Id == Id) ?? new Request();
        }
        public void DeleteRequest(int Id)
        {
            Request? studentRequest = Context.Requests.SingleOrDefault(r => r.Id == Id);
            if(studentRequest != null)
            {
                Context.Requests.Remove(studentRequest!);
                Context.SaveChanges();
            }
            
            
        }
    }
}