namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Trip
    {
        public int TripId { get; set; }
        public int RouteId { get; set; }    
        public Route? Route { get; set; }
        public ICollection<Student>? Students { get; set; }
        public int StudentId { get; set; }
        public DateTime Departure { get; set; } = DateTime.Now;
        public DateTime Arrival { get; set; }


    }
}
