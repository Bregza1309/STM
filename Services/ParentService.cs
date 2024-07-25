using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Server.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace STM.Services
{
    public class ParentService : IService<Parent>
    {
        UserManager<ApplicationUser> UserManager;
        RequestService _requestService;
        StmContext Context;
        public ParentService(UserManager<ApplicationUser> userManager , StmContext stmContext,RequestService requestService)
        {
            this.UserManager = userManager;
            this.Context = stmContext;
            this._requestService = requestService;
        }
        public void Create(Parent entity)
        {
            Parent? existing = Context.Parents.SingleOrDefault(p => p.UserId == entity.UserId);
            if (existing == null)
            {
                Context.Parents.Add(entity);
                Context.SaveChanges();
            }

        }

        public void Delete(string Id)
        {
            var parent = Get(Id).First();

            //Delete all the students associated with the parent
            Context.Students.RemoveRange(parent.Value.Students);

            //Delete all Requests associated with the parent
            Context.Requests.RemoveRange(parent.Value.Requests);

            //Delete the parent entities
            Context.Parents.Remove(parent.Value);
            Context.SaveChanges();

            UserManager.DeleteAsync(parent.Key).Wait();
        }

        public Dictionary<ApplicationUser, Parent> Get(string? userId = null)
        {
            Dictionary<ApplicationUser, Parent> result = new Dictionary<ApplicationUser, Parent>();
            if (string.IsNullOrEmpty(userId))
            {
                List<Parent> parents = Context.Parents.Include(p => p.Students).Include(p => p.Requests).OrderBy(p => p.UserId).ToList();
                List<ApplicationUser> applicationUsers = UserManager.GetUsersInRoleAsync("Parent").Result.OrderBy(p => p.Id).ToList();
                for(int i = 0 ; i < parents.Count; i++)
                {
                    result.Add(applicationUsers[i], parents[i]);
                }
            }
            else
            {
                ApplicationUser applicationUser = UserManager.FindByIdAsync(userId).Result;
                Parent parent = Context.Parents.Include(p => p.Students).Include(p => p.Requests).SingleOrDefault(p => p.UserId == userId) ?? new Parent();
                result.Add(applicationUser, parent);
            }
            return result;
        }

        public Dictionary<ApplicationUser, Parent> Search(string key)
        {
            var parents = Get();
            Regex rx = new Regex(@$"{key.ToLower()}");
            foreach (var kvp in parents)
            {
                var appUser = kvp.Key;
                if (!rx.IsMatch(appUser.Firstname.ToLower()) && !rx.IsMatch(appUser.Firstname.ToLower())
                    && !rx.IsMatch(appUser.UserName.ToLower()))
                {
                    parents.Remove(kvp.Key);
                }
            }
            return parents;
        }

        public void Update(Parent entity)
        {
            throw new NotImplementedException();
        }
        
    }
}
