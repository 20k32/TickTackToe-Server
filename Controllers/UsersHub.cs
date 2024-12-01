using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Server.Core.GamePool;
using Shared.Api.Messages;

namespace Server.Controllers
{
    public class UsersHub : Hub
    {
        private GamePool _gamePool;

        public UsersHub(IServiceProvider provider)
        {
            _gamePool = provider.GetService<GamePool>();
        }

        public void foo(string message) 
        {
            Clients.All.SendAsync("Receive", message);
        }

        public async void bar(string message, string to)
        {
            if(Context.UserIdentifier is string userId)
            {
                await Clients.Users(to, userId).SendAsync("OnStartMessage", message, userId);
            }
        }

        public Task AddToPoolAsync(string userName, string userId)
        {
            return _gamePool.AddUserAsync(userName, userId);
        }

        public async Task GameHappening(GameChangedParameter parameter)
        {
            if (Context.UserIdentifier is string userId)
            {
                await Clients.Users(parameter.ToUserId).SendAsync("OnGameHappening", parameter);
            }
        }

        public async Task ExitRequest(string toUserId)
        {
            if (Context.UserIdentifier is string userId)
            {
                await Clients.Users(toUserId).SendAsync("OnExitRequested", default(bool));
            }
        }

        public async Task ExitRequestApproved(string toUserId)
        {
            if (Context.UserIdentifier is string userId)
            {
                await Clients.Users(toUserId).SendAsync("OnExitApproved", default(bool));
            }
        }
    }
}
