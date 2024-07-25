using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STM.Services;
using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Shared.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace STM.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        IStudentRepository StudentRepository { get; set; }
        RequestService RequestService { get; set; }
        IService<Driver> DriverService { get; set; }
        VehicleRepository VehicleRepository { get; set; }
        UserManager<ApplicationUser> UserManager { get; set; }
        VerificationService VerificationService { get; set; }
        MessageRepository MessageRepository { get; set; }
        INotyfService NotyfService { get; set; }
        SchoolRepository SchoolRepository { get; set; }
        public DriverController(RequestService requestService, IService<Driver> driverService, 
            VehicleRepository vehicleRepository, UserManager<ApplicationUser> UserManager, 
            VerificationService verificationService,MessageRepository messageRepository,INotyfService notyfService, IStudentRepository StudentRepository ,
            SchoolRepository schoolRepository )
        {
            this.RequestService = requestService;
            this.DriverService = driverService;
            VehicleRepository = vehicleRepository;
            this.UserManager = UserManager;
            VerificationService = verificationService;
            this.MessageRepository = messageRepository;
            this.NotyfService = notyfService;
            this.StudentRepository = StudentRepository;
            this.SchoolRepository = schoolRepository;
        }
        public IActionResult Dashboard()
        {
            ViewData["page"] = "dashboard";
            return View();
        }
        public IActionResult Vehicles(string Id)
        {
            ViewData["page"] = "vehicles";
            return View(DriverService.Get(Id).Values.First().Vehicles.ToList());
        }
        [HttpGet]
        public IActionResult Driverslist(string? key)
        {
            ViewData["page"] = "driverslist";
            if(key != null)
            {
                return View(DriverService.Search(key));
            }
            return View(DriverService.Get());
        }
        public IActionResult LatestNews()
        {
            ViewData["page"] = "news";
            return View();
        }
        [HttpGet]
        public IActionResult VehicleEditor(int? Id)
        {
            ViewData["page"] = "vehicles";
            if (Id != null)
            {
                NotyfService.Information("Vehicle Edit Page",2);
                ViewBag.mode = "Edit";
                return View(VehicleRepository.Get(Id.Value));
            }
            NotyfService.Information("Vehicle Register Page", 2);
            ViewBag.mode = "Register";
            return View(new Vehicle());
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult VehicleEditor(Vehicle vehicle , IFormFile? file , string userId , string mode)
        {
            if (ModelState.IsValid )
            {
                if(file != null && file.Length > 0)
                {
                    // Check if the uploaded file is an image
                    if (!IsImage(file.ContentType))
                    {
                        ModelState.AddModelError(string.Empty, "Only Image files are Accepted");
                        return View(vehicle);
                    }
                    try
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        
                        if (string.IsNullOrEmpty(vehicle.Image))
                        {
                            vehicle.Image = fileName;
                            
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "vehicles", fileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyToAsync(stream).Wait();
                            }
                        }
                        else
                        {
                            // Delete the existing file if it exists
                            string existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "vehicles", vehicle.Image);
                            if (System.IO.File.Exists(existingFilePath))
                            {
                                System.IO.File.Delete(existingFilePath);
                            }

                          
                            string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "vehicles", fileName);

                            using (var stream = new FileStream(newFilePath, FileMode.Create))
                            {
                                file.CopyToAsync(stream).Wait();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, $"Internal Error : Contact the admin");
                        
                    }
                }
                if (VehicleRepository.IsRegistered(vehicle.RegistrationNumber))
                {
                    NotyfService.Success("Vehicle Updated", 2);

                    VehicleRepository.Update(vehicle);

                }
                else
                {
                    NotyfService.Success("Vehicle Added", 2);
                    VehicleRepository.Add(vehicle);
                }
                return Redirect($"/Driver/Vehicles/{userId}");

            }
            NotyfService.Error("Validation failed", 2);
            ViewBag.mode = mode;
            ViewData["page"] = "vehicles";
            return View(vehicle);


            //Inner function to check if content type is image
            bool IsImage(string contentType)
            {
                return contentType.StartsWith("image/");
            }
        }
        [HttpGet]
        public RedirectResult DeleteVehicle(int vehicleId , string userId)
        {
            VehicleRepository.Delete(vehicleId);
            NotyfService.Success("Vehicle Deleted", 2);
            return Redirect($"/Driver/Vehicles/{userId}");
        }
        public IActionResult Schools(string? key)
        {

            ViewData["page"] = "schools";
            if (key != null)
            {
                return View(SchoolRepository.Search(key));
            }
            return View(SchoolRepository.GetAll());
        }
        public IActionResult StudentsDetails(string Id , string? key)
        {
            
            ViewData["page"] = "students";
            ViewData["Id"] = Id;
            if(key != null)
            {
                NotyfService.Warning($"Search Results For '{key}'", 2);
                return View(DriverService.SearchStudents(key,Id));
            }
            return View(DriverService.Get(Id).Values.First().Students.ToList());
        }
        public IActionResult Requests(string Id , int? requestId , bool? decision)
        {
            ViewData["page"] = "requests";
            if(requestId != null && decision != null)
            {
                Request request = RequestService.Get(requestId.Value);
                request.Accepted = decision.Value;
                RequestService.ProcessRequest(request);
                NotyfService.Success("Vehicle Updated", 2);
            }
            
            ViewData["Id"] = Id;
            return View(DriverService.Get(Id).Values.First().Requests);
        }
        [HttpGet]
        public IActionResult MyEditor(string id)
        {
            var user = UserManager.FindByIdAsync(id).Result;
            var driver = DriverService.Get(id).Values.First();
            return View(new DriverEditViewModel
            {
                Id = id,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                UserName = user.UserName,
                Address = user.Address,
                LicenceNumber = driver.LicenseNumber
            });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult MyEditor(DriverEditViewModel model, IFormFile? file)
        {

            var user = UserManager.FindByIdAsync(model.Id).Result;
            
            if (ModelState.IsValid)
            {
                bool usernameChanged = user.UserName != model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.Firstname = model.FirstName;
                user.Lastname = model.LastName;
                user.Email = model.Email;
                user.Address = model.Address;
                var driver = DriverService.Get(user.Id).Values.First();
                driver.LicenseNumber = model.LicenceNumber;
                user.UserName = model.UserName;

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

                        if (user.Avatar == "profile.png")
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
                            string existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", user.Avatar);
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
                        user.Avatar = fileName;
                        //Update the database
                        DriverService.Update(driver);
                        UserManager.UpdateAsync(user).Wait();
                        NotyfService.Success("Driver Updated", 2);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, $"Internal Error : Contact the admin");

                    }
                }
                else
                {
                    DriverService.Update(driver);
                    UserManager.UpdateAsync(user).Wait();
                    NotyfService.Success("Driver Updated", 2);

                }
                if (model.Password != null && model.ConfirmPassword != null)
                {
                    if (model.CurrentPassword != null)
                    {
                        if (model.Password == model.ConfirmPassword)
                        {
                            var result = UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password).Result;
                            if (!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                            else
                            {
                                NotyfService.Warning("Password Updated please re-login", 2);

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
                if (usernameChanged)
                {
                    NotyfService.Warning("Username Updated please re-login", 2);
                    return Redirect("/Account/Logout");
                }

                return Redirect($"/Driver/Dashboard");
            }
            ModelState.AddModelError(string.Empty, $"Fill In The Required Information");
            model.Avatar = user.Avatar;
            return View(model);

            //Inner function to check if content type is image
            bool IsImage(string contentType)
            {
                return contentType.StartsWith("image/");
            }
        }
        public RedirectResult SendVerification(int driverId , int vehicleId)
        {
            VerificationService.Add(new Verification
            {
                DriverId = driverId,
                VehicleId = vehicleId
            });
            NotyfService.Success("Vehicle Verification Sent", 2);
            return Redirect("/Driver/Vehicles");
        }
        public IActionResult Verifications(string driverId,int? Id)
        {
            if(Id != null)
            {
                VerificationService.Remove(Id.Value);
                NotyfService.Success("Vehicle Verification Removed", 2);
            }
            ViewData["page"] = "verify";
            return View(VerificationService.Get(driverId));
        }
        public IActionResult Contacts()
        {
            
            var driver = UserManager.FindByNameAsync(User.Identity!.Name!).Result;
            var messages = MessageRepository.GetMessages(driver.Id);
            List<ApplicationUser> users = messages.Select(m => UserManager.FindByIdAsync(m.Recipient_Id).Result).ToList();
            users.Insert(0, UserManager.Users.ToArray().Single(u => UserManager.IsInRoleAsync(u, "admin").Result));
            users = users.Distinct().ToList();
            users.Remove(UserManager.FindByIdAsync(driver.Id).Result);
            NotyfService.Information("Contacts Page \nClick in a contact to chat", 2);
            return View(users);
        }
        public RedirectResult CheckIn(int Id, string parentId, bool checkIn)
        {
            Student student = StudentRepository.GetStudent(Id);
            if (checkIn)
            {
                student.Status = "Stationery(School)";
            }
            else
            {
                student.Status = "Collected";
            }
            StudentRepository.Update(student);
            return Redirect($"/Driver/StudentsDetails?Id={parentId}");
        }
    }
}
