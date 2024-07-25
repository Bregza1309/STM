using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class DriverEditViewModel:AdminEditViewModel
    {
        [Required(ErrorMessage = "Licence Number is Required")]
        public string LicenceNumber { get; set; } = string.Empty;
        
    }
}
