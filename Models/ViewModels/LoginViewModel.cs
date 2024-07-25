using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is Required")]
        public string Username { get; set; }  = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; } = "/";
    }
}
