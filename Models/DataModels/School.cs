using System.ComponentModel.DataAnnotations;
namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Provice { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    }
}
