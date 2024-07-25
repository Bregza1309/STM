using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vehicle Description is required")]
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vehicle RegistrationNumber is required")]
        public string RegistrationNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vehicle Capacity is required")]
        public int StudentCapacity { get; set; }
        public Driver? Driver { get; set; }
        public int DriverId { get; set; }
        public ICollection<Verification> Verifications { get; set; } = new List<Verification>();
    }
}
