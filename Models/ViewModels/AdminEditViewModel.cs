using System.ComponentModel.DataAnnotations;

namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class AdminEditViewModel
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Firstname is Required")]
        [RegularExpression(@"^[a-zA-Z]{3,40}$", ErrorMessage = "Only Alphabetical Characters are allowed")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lastname is Required")]
        [RegularExpression(@"^[a-zA-Z]{3,40}$", ErrorMessage = "Only Alphabetical Characters are allowed")]
        public string LastName { get; set; } = string.Empty;
        [RegularExpression(@"^[a-zA-Z]{3,40}$", ErrorMessage = "Only Alphabetical Characters are allowed")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        public string Avatar { get; set; } = string.Empty;
        public string? CurrentPassword { get; set; }
        public string? Password { get; set; } 
        
        public string? ConfirmPassword { get; set; }
    }
}
