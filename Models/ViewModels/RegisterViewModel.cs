using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Firstname is Required")]
        [RegularExpression(@"^[a-zA-Z]{3,40}$" ,ErrorMessage ="Only Alphabetical Characters are allowed")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage ="Lastname is Required")]
        [RegularExpression(@"^[a-zA-Z]{3,40}$" ,ErrorMessage ="Only Alphabetical Characters are allowed")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9]{9}$")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Role is Required")]
        public string Role { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please Confirm Your Password")]
        public string ConfirmPassword { get; set; } = string.Empty; 
        [Required(ErrorMessage = "Please Confirm Your Address")]
        public string Address { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        

    }
}
