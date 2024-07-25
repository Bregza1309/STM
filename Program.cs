using Microsoft.AspNetCore.Identity;
using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Server.Context;
using static StudentTransportManagement_STM_.Server.SeedData.DataSeeding;
using STM.Services;
using static StudentTransportManagement_STM_.Server.SeedData.IdentitySeedData;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using STM.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

//Add IdentityContext service
builder.Services.AddDbContext<IdentityContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConn"));
    
    opts.EnableSensitiveDataLogging();
});

//add signal R support
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
       new[] { "application/octet-stream" });
});
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
//add identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();
builder.Services.AddNotyf(opts =>
{
    opts.DurationInSeconds = 10;
    opts.IsDismissable = true;
    opts.Position = NotyfPosition.TopCenter;
});
//add add StmContext service
builder.Services.AddDbContext<StmContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DataConn"));
    opts.EnableSensitiveDataLogging();
});

//ADD driver service
builder.Services.AddScoped<IService<Driver>, DriverService>();

//add request service
builder.Services.AddScoped<RequestService>();
//add parent service
builder.Services.AddScoped<IService<Parent>, ParentService>();
builder.Services.AddControllersWithViews();

//add schools service
builder.Services.AddScoped<SchoolRepository>();


//add Student Repository
builder.Services.AddScoped<IStudentRepository,StudentRepository>();

//add parent reposiory
builder.Services.AddScoped<ParentRepository, ParentRepository>();

//add Vehicle Service
builder.Services.AddScoped<VehicleRepository>();

//Add Verifications Service
builder.Services.AddScoped<VerificationService>();
//Add Message repository
builder.Services.AddScoped<MessageRepository>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseNotyf();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chathub");
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/map/{*catchall}","/MapIndex");
app.MapFallbackToPage("/chat/{*catchall}", "/Index");
//Add seeding data
EnsureIdentitySeeding(app);
EnsureDataSeeding(app);
app.Run();
