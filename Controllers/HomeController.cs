using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using STM.Models;
using STM.Services;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Shared.ViewModels;
using System.Diagnostics;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace STM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        VerificationService VerificationService { get; set; }
        INotyfService NotyfService { get; set; }
        UserManager<ApplicationUser> UserManager { get; set; }
        public HomeController(ILogger<HomeController> logger, VerificationService verificationService, INotyfService notyfService, UserManager<ApplicationUser> UserManager )
        {
            _logger = logger;
            VerificationService = verificationService;
            NotyfService = notyfService;
            this.UserManager = UserManager;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Verify(string Id)
        {
            if (VerificationService.SendVerificationSms(Id))
            {
                NotyfService.Success("Validation Successfull", 3);
                NotyfService.Warning("Waiting for Verification ...........", 4);

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
            if (ModelState.IsValid)
            {
                if (VerificationService.VerifyUser(model.PhoneNumber, model.Otp))
                {
                    ApplicationUser? user = UserManager.Users.SingleOrDefault(u => u.PhoneNumber == model.PhoneNumber);
                    if (user != null)
                    {
                        user.PhoneNumberConfirmed = true;
                        UserManager.UpdateAsync(user).Wait();
                        NotyfService.Success("Phonenumber Verified Successfully", 3);
                        return Redirect("/Account/Login");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Incorrect OTP");
                }
            }
            NotyfService.Error("Validation Failed", 3);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}