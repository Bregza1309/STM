using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.ViewModels;
using STM.Services;
using Microsoft.AspNetCore.Identity;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace STM.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        IService<Driver> DriverService;
        IService<Parent> ParentService;
        IStudentRepository StudentRepository;
        UserManager<ApplicationUser> UserManager;
        VerificationService VerificationService;
        MessageRepository MessageRepository;
        INotyfService NotyfService;
        SchoolRepository SchoolRepository;

        public AdminController(IService<Driver> driversService , IService<Parent> parentService, 
            IStudentRepository studentRepository, UserManager<ApplicationUser> UserManager,
            VerificationService verificationService,MessageRepository messageRepository,INotyfService notyfService,SchoolRepository schoolRepository)
        {
            this.DriverService = driversService;
            ParentService = parentService;
            this.UserManager = UserManager;
            StudentRepository = studentRepository;
            this.VerificationService = verificationService;
            this.MessageRepository = messageRepository;
            this.NotyfService = notyfService;
            this.SchoolRepository = schoolRepository;
        }
        public IActionResult Dashboard()
        {
            ViewData["page"] = "dashboard";
            
            return View();
        }
        public IActionResult Driverdetails(string userId)
        {
            ViewData["page"] = "driverslist";
            return View(DriverService.Get(userId));
        }
        [HttpGet]
        public IActionResult Driverslist(string? key)
        {
            ViewData["page"] = "driverslist";
            if (key != null)
            {
                NotyfService.Information($"Search Results for '{key}'", 2);
                return base.View(DriverService.Search(key));
            }
            return base.View(DriverService.Get());
        }
       
        
        public IActionResult Parents( string? key)
        {
            ViewData["page"] = "parents";
            if(key != null)
            {
                NotyfService.Information($"Search Results for '{key}'", 2);
                return View(ParentService.Search(key));
            }
            return View(ParentService.Get());
        }
        public IActionResult Schools()
        {
            ViewData["page"] = "schools";
            return View();
        }
        [HttpPost]
        public IActionResult Schools(SchoolViewModel model)
        {
            if (ModelState.IsValid)
            {
                SchoolRepository.Add(new School
                {
                    Name = model.Name,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Provice = model.Provice,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                });
                NotyfService.Success("School successfully added",3);
                return Redirect("/admin/dashboard");
            }
            return View();
        }
        public IActionResult StudentsDetails(string? driverId, string? parentId, string? key)
        {

            ViewData["page"] = "students";
            if(driverId != null)
            {
                return View(DriverService.Get(driverId).Values.First().Students.ToList());
            }
            if(parentId != null)
            {
                return View(ParentService.Get(parentId).Values.First().Students.ToList());
            }
            if (key != null)
            {
                return View(StudentRepository.Search(key));
            }
            return View(StudentRepository.GetAll());
        }
        public RedirectResult DeleteDriver(string Id)
        {
            DriverService.Delete(Id);
            NotyfService.Success("Driver deleted",3);
            return Redirect("/Admin/Driverslist");
        }
        public RedirectResult DeleteParent(string Id)
        {
            ParentService.Delete(Id);
            NotyfService.Success("Parent Deleted", 3);
            return Redirect("/Admin/Parents");
        }
        [HttpGet]
        public IActionResult MyEditor(string id)
        {
            var admin = UserManager.FindByIdAsync(id).Result;
            return View(new AdminEditViewModel
            {
                Id = id,
                FirstName  = admin.Firstname,
                LastName = admin.Lastname,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Avatar  = admin.Avatar,
                UserName = admin.UserName,
            });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult MyEditor(AdminEditViewModel model, IFormFile? file)
        {
            
            var adminUser = UserManager.FindByIdAsync(model.Id).Result;
            if (ModelState.IsValid)
            {
                bool usernameChanged = adminUser.UserName != model.UserName;
                adminUser.PhoneNumber = model.PhoneNumber;
                adminUser.Firstname = model.FirstName;
                adminUser.Lastname = model.LastName;
                adminUser.Email = model.Email;

                adminUser.UserName = model.UserName;

                if (file != null && file.Length > 0)
                {
                    if (!IsImage(file.ContentType))
                    {
                        ModelState.AddModelError(string.Empty, "Only Image files are Accepted");
                        return View();
                    }
                    try
                    {
                        string fileName = Path.GetFileName(file.FileName);

                        if (adminUser.Avatar == "profile.png")
                        {

                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", fileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyToAsync(stream).Wait();
                            }


                        }
                        else
                        {
                            // Delete the existing file if it exists
                            string existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", adminUser.Avatar);
                            if (System.IO.File.Exists(existingFilePath))
                            {
                                System.IO.File.Delete(existingFilePath);
                            }


                            string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", fileName);

                            using (var stream = new FileStream(newFilePath, FileMode.Create))
                            {
                                file.CopyToAsync(stream).Wait();
                            }
                        }
                        adminUser.Avatar = fileName;
                        UserManager.UpdateAsync(adminUser).Wait();
                        NotyfService.Success("User Information Updated", 3);


                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, $"Internal Error : Contact the admin");

                    }
                }
                else
                {
                    UserManager.UpdateAsync(adminUser).Wait();
                    NotyfService.Success("User Information Updated", 3);
                }
                if (model.Password != null && model.ConfirmPassword != null)
                {
                    if (model.CurrentPassword != null)
                    {
                        if (model.Password == model.ConfirmPassword)
                        {
                            var result = UserManager.ChangePasswordAsync(adminUser, model.CurrentPassword, model.Password).Result;
                            if (!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                            else
                            {
                                return Redirect("/Account/Logout");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Password Confirmation Incorrect");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Enter Current Password to change Password");
                    }

                }
                if(usernameChanged)
                {
                    NotyfService.Information("Username was Updated Please login again", 3);
                    return Redirect("/Account/Logout");
                }
                return Redirect($"/Admin/Dashboard");
            }
            ModelState.AddModelError(string.Empty, $"Fill In The Required Information");
            model.Avatar = adminUser.Avatar;
            return View(model);

            //Inner function to check if content type is image
            bool IsImage(string contentType)
            {
                return contentType.StartsWith("image/");
            }
        }
        public IActionResult Verifications(int? Id, bool? decision)
        {
            if(Id != null && decision != null)
            {
                Verification verification = VerificationService.Get().Single(v => v.Id == Id);
                verification.Verified = decision;
                if (decision.Value)
                {
                    verification.Driver!.Verified = true;
                }
                VerificationService.Update(verification);
            }
            ViewData["page"] = "verify";
            return View(VerificationService.Get());
        }
        public IActionResult Contacts()
        {
            var driver = UserManager.FindByNameAsync(User.Identity!.Name!).Result;
            var messages = MessageRepository.GetMessages(driver.Id);
            List<ApplicationUser> users = messages.Select(m => UserManager.FindByIdAsync(m.Recipient_Id).Result).ToList();
            users = users.Distinct().ToList();
            users.Remove(UserManager.FindByIdAsync(driver.Id).Result);
            NotyfService.Information("Contacts Page \nClick in a contact to chat", 2);
            return View(users);
        }




    }
}
