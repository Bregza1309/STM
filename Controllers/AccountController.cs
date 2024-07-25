using Microsoft.AspNetCore.Mvc;
using StudentTransportManagement_STM_.Shared.ViewModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using STM.Services;
using StudentTransportManagement_STM_.Shared.DataModels;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace STM.Controllers
{
    public class AccountController : Controller
    {
        SignInManager<ApplicationUser> SignInManager { get; set; }
        UserManager<ApplicationUser> UserManager { get; set; }
        IService<Parent> ParentServices { get; set; }
        IService<Driver> DriverServices { get; set; }
        VerificationService VerificationService { get;set; }
        INotyfService NotyfService { get; set; }
        
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IService<Driver> driverServices, IService<Parent> parentService, VerificationService verification,INotyfService notyfService)
        {
            this.SignInManager = signInManager;
            this.UserManager = userManager;
            DriverServices = driverServices;
            ParentServices = parentService;
            VerificationService = verification;
            NotyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            
            NotyfService.Information("Username is a combination of Firstname and Lastname for New Users");
            return View();
        }
        //Implementation of App user Login logic
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByNameAsync(model.Username);
                if (user != null)
                {   
                    if(user.PhoneNumberConfirmed == false)
                    {
                        ModelState.AddModelError(string.Empty , "Your PhoneNumber is not Verified");
                        ViewData["mode"] = "verify";
                        ViewBag.phoneNumber = user.PhoneNumber;
                        ViewBag.Id = user.Id;
                        NotyfService.Information("Enter your PhoneNumber below for Verification");
                        NotyfService.Warning("Start after '+27' eg '123456789' ");
                        return View();
                    }
                    Microsoft.AspNetCore.Identity.SignInResult result = await SignInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        if(await UserManager.IsInRoleAsync(user, "admin"))
                        {
                            NotyfService.Success($"Welcome {user.UserName}");
                            return Redirect("/admin/dashboard");
                        }
                        if (await UserManager.IsInRoleAsync(user, "parent"))
                        {
                            NotyfService.Success($"Welcome {user.UserName}");
                            return Redirect("/parent/dashboard");
                        }
                        if (await UserManager.IsInRoleAsync(user, "driver"))
                        {
                            NotyfService.Success($"Welcome {user.UserName}");
                            return Redirect("/driver/dashboard");
                        }
                    }

                    ModelState.AddModelError("","Password is Incorrect");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username does not exist");
                }
            }
            NotyfService.Error("Validation Failed", 3);
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            NotyfService.Information("New User Registration",2);
            NotyfService.Warning("Fill in all the Fields",3);
            return View();
        }
        //implementation of the app user registration
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser user = new() { 
                    Firstname = model.FirstName, Lastname = model.LastName, Email = model.Email,
                    PhoneNumber = $"+27{model.PhoneNumber}" , UserName = $"{model.FirstName}{model.LastName}"
                    ,Address = model.Address,Latitude = model.Latitude,Longitude = model.Longitude
                };
                IdentityResult result = UserManager.CreateAsync(user,model.Password).Result;
                if(result.Succeeded)
                {
                    result = UserManager.AddToRoleAsync(user, model.Role).Result;
                    if (result.Succeeded)
                    {
                        if(model.Role == "Parent")
                        {
                            ParentServices.Create(new Parent() { UserId = user.Id });
                            
                        }
                        else
                        {
                            DriverServices.Create(new Driver() { UserId = user.Id});
                        }

                        return Redirect($"/home/verify/{user.PhoneNumber}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,$"User can not be added to {model.Role}s \nContact the Admin");
                    }
                }
                else
                {
                    foreach(IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty,err.Description);
                    }
                }
            }
            ModelState.AddModelError(string.Empty,"Please fill in the missing or Invalid Information");
            NotyfService.Error("Validation Failed", 3);
            return View(model);
        }
        public RedirectResult Logout()
        {
            SignInManager.SignOutAsync().Wait();
            return Redirect("/account/login");
        }
        public IActionResult Verify(string Id)
        {
            if (VerificationService.SendVerificationSms(Id))
            {
                NotyfService.Success("Validation Successfull",3);
                NotyfService.Warning("Waiting for Verification ...........",4);

                return View(new OTPViewModel
                {
                    PhoneNumber = Id
                });
            }
            return View();
            
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Verify(OTPViewModel model)
        {
            if(ModelState.IsValid)
            {
                if (VerificationService.VerifyUser(model.PhoneNumber, model.Otp))
                {
                    ApplicationUser? user = UserManager.Users.SingleOrDefault(u => u.PhoneNumber == model.PhoneNumber);
                    if (user != null)
                    {
                        user.PhoneNumberConfirmed = true;
                        UserManager.UpdateAsync(user).Wait();
                        NotyfService.Success("Phonenumber Verified Successfully",3);
                        return Redirect("/Account/Login");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Incorrect OTP");
                }
            }
            NotyfService.Error("Validation Failed",3);
            return View(model);
        }
        [HttpGet]
        public RedirectResult UpdatePhone(string Id ,string userId) { 
            ApplicationUser user = UserManager.FindByIdAsync(userId).Result;
            user.PhoneNumber = Id;
            UserManager.UpdateAsync(user).Wait();
            return Redirect($"/home/verify/{user.PhoneNumber}");
        }
    }
}
