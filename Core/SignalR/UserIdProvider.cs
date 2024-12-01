using Microsoft.AspNetCore.SignalR;
using Server.Core.Auth;

namespace Server.Core.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection) 
            => connection?.User.FindFirst(CustomClaimTypes.USER_ID)?.Value;
    }
}
