using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class OTPViewModel
    {
        [Required(ErrorMessage = "Enter the OTP received in your SMSs")]
        [RegularExpression(@"^[0-9]{6}$",ErrorMessage ="OTP should be 6 numerical digits")]
        public string Otp { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        

    }
}
