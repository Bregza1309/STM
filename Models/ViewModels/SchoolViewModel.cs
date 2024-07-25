using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.ViewModels
{
    public class SchoolViewModel
    {
        [Required(ErrorMessage ="School's Name is Required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "City's Name is Required")]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "Province's Name is Required")]
        public string Provice { get; set; } = string.Empty;
        [Required(ErrorMessage = "Postal's Code is Required")]
        public string PostalCode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string? Longitude { get; set; } = string.Empty;
    }
}
