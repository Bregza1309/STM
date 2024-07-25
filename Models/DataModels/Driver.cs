namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Request> Requests { get; set; } = new List<Request>();   
        public bool Verified { get; set; } = false;
    }
}
