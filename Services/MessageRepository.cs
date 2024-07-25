using StudentTransportManagement_STM_.Shared;
using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;

namespace STM.Services
{
    public class MessageRepository
    {
        StmContext Context { get; set; }
        public MessageRepository(StmContext context)
        {
            this.Context = context;
        }
        public List<Message> GetMessages(string userId ) =>
            Context.Messages.Where(m => m.Recipient_Id == userId || m.Sender_Id == userId).OrderBy(m => m.DateTime).ToList();
        
        public void SaveMessage(Message message)
        {
            Context.Messages.Add(message);
            Context.SaveChanges();
        }

    }
}
