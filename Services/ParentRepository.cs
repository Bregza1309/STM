using StudentTransportManagement_STM_.Server.Context;
using Microsoft.AspNetCore.Identity;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Shared.DataModels;
using Microsoft.EntityFrameworkCore;
namespace STM.Services
{
    public class ParentRepository
    {
        StmContext Context { get; set; }
        UserManager<ApplicationUser> UserManager { get; set; }

        public ParentRepository(StmContext context , UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }
        public Parent GetParent(string username)
        {
            ApplicationUser user = UserManager.FindByNameAsync(username).Result;
            Parent parent = Context.Parents.Include(P => P.Requests).SingleOrDefault( p => p.UserId == user.Id) ?? new Parent();
            return parent;

        }
        public void  AddStudent(Student student , int Id)
        {
            Parent parent = Context!.Parents.SingleOrDefault( p => p.ParentId == Id) ?? new Parent();
            parent.Students!.Add(student);
            Context.SaveChanges();

        }
    }
}
