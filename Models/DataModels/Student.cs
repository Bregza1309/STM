namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int? DriverId { get; set; }
        public int? ParentId { get; set; }
        public Driver? Driver { get; set; } 
        public Parent? Parent { get; set; }
        public School? School { get; set; }
        public int? SchoolId { get; set; }   
        public int TripId { get;set; }
        public ICollection<Trip>? MyTrips { get; set; }
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string? Status { get; set; }
        public bool CheckedIn { get; set; } = false;
    }
}
