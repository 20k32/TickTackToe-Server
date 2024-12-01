using Microsoft.AspNetCore.SignalR;

namespace Server.Core.SignalR
{
    public static class DiExtensions
    {
        public static IServiceCollection AddUserIdProvider(this IServiceCollection collection)
            => collection.AddSingleton<IUserIdProvider, UserIdProvider>();
    }
}
