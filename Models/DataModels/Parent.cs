namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Parent
    {
        public int ParentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Request> Requests { get; set; }  = new List<Request>();
    }
}
