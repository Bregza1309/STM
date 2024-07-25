namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Verification
    {
        public int Id { get; set; }
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
        public bool? Verified { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
