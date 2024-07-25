namespace StudentTransportManagement_STM_.Shared.DataModels
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Sender_Id { get; set; } = string.Empty;
        public string Recipient_Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

    }
}
