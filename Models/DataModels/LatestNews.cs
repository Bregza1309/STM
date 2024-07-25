namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class LatestNews
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; }
    }
}
