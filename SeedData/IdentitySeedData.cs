using StudentTransportManagement_STM_.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
namespace StudentTransportManagement_STM_.Server.SeedData
{
    public static class IdentitySeedData
    {
        public static void EnsureIdentitySeeding(IApplicationBuilder app)
        {
            UserManager<ApplicationUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "Parent", "Driver" };
            if (!roleManager.Roles.Any())
            {
                //Create Default  roles 
                IdentityRole[] identityRoles = roles.Select(roles => new IdentityRole(roles)).ToArray();
                foreach (var role in identityRoles)
                {
                    roleManager.CreateAsync(role).Wait();
                }
            }
            if(!userManager.Users.Any())
            {
                //add default users
                string[] userNames = { "JohnBlink","TrayWright","AndreSmith"};
                Dictionary<string, string> Users = new Dictionary<string, string>()
                {
                    {"John","Blink" },{"Tray","Wright"},{"Andre","Smith"}
                };
                List<ApplicationUser> applicationUsers = Users.Select(kvp => new ApplicationUser
                {
                    Firstname = kvp.Key,
                    Lastname = kvp.Value,
                    PhoneNumberConfirmed = true
                }).ToList();
                for(int i = 0; i < applicationUsers.Count; i++)
                {
                    applicationUsers[i].UserName = userNames[i];
                }
                
                foreach (var user in applicationUsers)
                {
                    userManager.CreateAsync(user, "Pa$$w0rd").Wait();
                }
                //add users to roles
                for (int i = 0; i < applicationUsers.Count; i++)
                {
                    userManager.AddToRoleAsync(applicationUsers[i], roles[i]).Wait();
                }
            }
           

        }
    }
}
