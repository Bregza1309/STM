using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STM.Services;
using StudentTransportManagement_STM_.Shared.DataModels;
using StudentTransportManagement_STM_.Shared.IdentityModel;
using StudentTransportManagement_STM_.Shared.ViewModels;
using System.Text.RegularExpressions;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace STM.Controllers
{
    [Authorize(Roles = "Parent")]
    public class ParentController : Controller
    {
        IStudentRepository StudentRepository { get; set; }
        ParentRepository ParentRepository { get; set; }   
        IService<Driver> DriverService { get; set; }
        IService<Parent> ParentService { get; set; }
        RequestService RequestService { get; set; }
        UserManager<ApplicationUser> UserManager { get; set; }
        MessageRepository MessageRepository { get; set; }
        INotyfService NotyfService { get; set; }
        SchoolRepository SchoolRepository { get; set; }
        public ParentController(IStudentRepository studentRepo, ParentRepository parentRepository
                ,IService<Driver> driverService, RequestService requestService,IService<Parent> parentService, 
            UserManager<ApplicationUser> UserManager,MessageRepository messageRepository,INotyfService notyfService,
            SchoolRepository schoolRepository)
        {
            this.StudentRepository = studentRepo;
            ParentRepository = parentRepository;
            this.RequestService = requestService;
            this.DriverService = driverService;
            this.ParentService = parentService;
            this.UserManager = UserManager;
            this.MessageRepository = messageRepository;
            this.NotyfService = notyfService;
            this.SchoolRepository = schoolRepository;

        }
        public IActionResult Dashboard()
        {
            ViewData["page"] = "dashboard";
            return View();
        }
        public IActionResult Driverdetails(string userId,int studentId , int parentId)
        {
            ViewData["StudentId"] = studentId.ToString();
            ViewData["ParentId"] = parentId.ToString();
            ViewData["page"] = "driverslist";
            return View(DriverService.Get(userId));
        }
        public IActionResult Driverslist(int studentId , int parentId , string? key)
        {
            ViewData["StudentId"] = studentId.ToString();
            ViewData["ParentId"] = parentId.ToString();
            ViewData["page"] = "driverslist";
            if(key != null)
            {
                NotyfService.Information($"Showing Results for '{key}'", 2);
                return View(DriverService.Search(key));
            }
            return View(DriverService.Get());
        }
        
        public IActionResult DriverTracking()
        {
            ViewData["page"] = "tracking";
            return View();
        }
        public IActionResult LatestNews()
        {
            ViewData["page"] = "news";
            return View();
        }
        
        public IActionResult Schools(string? key)
        {
            
            ViewData["page"] = "schools";
            if(key != null)
            {
                return View(SchoolRepository.Search(key));
            }
            return View(SchoolRepository.GetAll());
        }
        public IActionResult StudentsDetails(string Id)
        {
            ViewData["page"] = "students";
            return View(ParentService.Get(Id).Values.First().Students.ToList());
        }
        [HttpGet]
        public IActionResult Editor(int? Id)
        {
            ViewData["page"] = "students";
            if (Id == null)
            {
                return View(new Student());
            }
            else
            {
                ViewData["mode"] = "edit";
                return View(StudentRepository.GetStudent(Id.Value));
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Editor(Student student , string name)
        {
            if(ModelState.IsValid)
            {
                Parent user = ParentRepository.GetParent(name);
                if (!StudentRepository.IsRegistered(student))
                {
                    
                    StudentRepository.Add(student,user.ParentId);
                    NotyfService.Success("Student Added",2);
                }
                else
                {
                    StudentRepository.Update(student);
                    NotyfService.Success("Student Updated", 2);
                }
                return Redirect($"/parent/studentsdetails/{user.UserId}");
            }
            ModelState.AddModelError(string.Empty,"Please fill in or Correct required fields");
            NotyfService.Error("Validation Failed", 2);
            return View(student);
        }
        [HttpGet]
        public RedirectResult Delete(int id)
        {
            var parent = StudentRepository.GetStudent(id).Parent!.UserId;
            StudentRepository.Delete(id);
            NotyfService.Success("Student Deleted" , 2);
            return Redirect($"/parent/studentsdetails/{parent}");
        }
        [HttpGet]
        public RedirectResult RequestDriver(int driverId, int studentId, int parentId)
        {
            
            RequestService.Request(new Request
            {
                ParentId = parentId,
                DriverId = driverId,
                StudentId = studentId
            });
            NotyfService.Success("Request Sent", 2);
            return Redirect("/parent/driverslist");

        }
        [HttpGet]
        public RedirectResult RevokeDriver(int Id)
        {
            Student student = StudentRepository.GetStudent(Id);
            student.DriverId = null;
            student.Driver = null;
            StudentRepository.Update(student,true);
            NotyfService.Success("Driver Revoked", 2);
            return Redirect($"/parent/studentsDetails/{student.Parent!.UserId}");
        }
        public IActionResult Requests(string username , int? Id)
        {
            if(Id != null)
            {
                RequestService.DeleteRequest(Id.Value);
                NotyfService.Success("Request Deleted", 2);
            }
            ViewData["page"] = "requests";
            return View(ParentRepository.GetParent(username).Requests.ToList());
        }

        [HttpGet]
        public IActionResult MyEditor(string id)
        {
            var admin = UserManager.FindByIdAsync(id).Result;
            return View(new AdminEditViewModel
            {
                Id = id,
                FirstName = admin.Firstname,
                LastName = admin.Lastname,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Avatar = admin.Avatar,
                UserName = admin.UserName,
                Address = admin.Address,
            });
        }
       

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult MyEditor(AdminEditViewModel model, IFormFile? file)
        {

            var parentUser = UserManager.FindByIdAsync(model.Id).Result;
            if (ModelState.IsValid)
            {
                bool usernameChanged = parentUser.UserName != model.UserName;
                parentUser.PhoneNumber = model.PhoneNumber;
                parentUser.Firstname = model.FirstName;
                parentUser.Lastname = model.LastName;
                parentUser.Email = model.Email;
                parentUser.Address = model.Address;

                parentUser.UserName = model.UserName;

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

                        if (parentUser.Avatar == "profile.png")
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
                            string existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", parentUser.Avatar);
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
                        parentUser.Avatar = fileName;
                        UserManager.UpdateAsync(parentUser).Wait();

                        NotyfService.Success("User Updated", 2);

                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, $"Internal Error : Contact the admin");

                    }
                }
                else
                {
                    UserManager.UpdateAsync(parentUser).Wait();
                    NotyfService.Success("User Updated", 2);

                }
                if (model.Password != null && model.ConfirmPassword != null)
                {
                    if (model.CurrentPassword != null)
                    {
                        if (model.Password == model.ConfirmPassword)
                        {
                            var result = UserManager.ChangePasswordAsync(parentUser, model.CurrentPassword, model.Password).Result;
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

                return Redirect($"/Parent/Dashboard");
            }
            ModelState.AddModelError(string.Empty, $"Fill In The Required Information");
            model.Avatar = parentUser.Avatar;
            NotyfService.Error("Validation Failed", 2);
            return View(model);

            //Inner function to check if content type is image
            bool IsImage(string contentType)
            {
                return contentType.StartsWith("image/");
            }
        }
        public IActionResult Contacts()
        {
            var parent = ParentRepository.GetParent(User.Identity!.Name!);
            var messages = MessageRepository.GetMessages(parent.UserId);
            List<ApplicationUser> users = messages.Select(m => UserManager.FindByIdAsync(m.Recipient_Id).Result).ToList();
            users.Insert(0,UserManager.Users.ToArray().Single(u => UserManager.IsInRoleAsync(u,"admin").Result));
            users = users.Distinct().ToList();
            users.Remove(UserManager.FindByIdAsync(parent.UserId).Result);
            NotyfService.Information("Contacts Page \nClick in a contact to chat", 2);
            return View(users);
        }
        public RedirectResult CheckIn(int Id , string parentId,bool checkIn)
        {
            Student student = StudentRepository.GetStudent(Id);
            student.CheckedIn = checkIn;
            if (checkIn)
            {
                student.Status = "In Transit";
            }
            else
            {
                student.Status = "Stationery(Home)";
            }
            StudentRepository.Update(student);
            return Redirect($"/Parent/StudentsDetails?Id={parentId}");
        }
    }
}
