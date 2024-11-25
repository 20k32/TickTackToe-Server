using Microsoft.AspNetCore.SignalR;

namespace Server.Controllers
{
    public class UsersHub : Hub
    {
        public void foo(string message) 
        {
            Clients.All.SendAsync("Receive", message);
        }
    }
}
