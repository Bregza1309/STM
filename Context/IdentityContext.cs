using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using Microsoft.EntityFrameworkCore;

namespace StudentTransportManagement_STM_.Server.Context
{
    public class IdentityContext:IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> opt):base(opt)
        {
            
        }
    }
}
