using Microsoft.AspNetCore.SignalR;
using StudentTransportManagement_STM_.Shared.DataModels;
namespace STM.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(string message , string senderId)
        {
            await Clients.All.SendAsync("ReceiveMessage", message , senderId);
        }
    }
}
