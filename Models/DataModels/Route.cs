namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Route
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string StartingPoint { get; set; } = string.Empty;
        public string EndingPoint { get; set; } = string.Empty;
        public ICollection<Trip>? TripList { get; set; }
        
    }
}
