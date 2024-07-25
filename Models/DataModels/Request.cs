namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Request
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public Parent? Parent { get; set; }
        public int DriverId { get; set; }   
        public Driver? Driver { get; set; } 
        public bool? Accepted { get; set; } 
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}
