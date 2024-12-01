using Microsoft.AspNetCore.SignalR;
using Server.Controllers;
using Server.Persistence.MongoDb;
using Shared.Api.Messages;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Server.Core.GamePool
{
    public class GamePool
    {
        //private static BlockingCollection<GamePoolItem> _collection;

        private readonly IServiceProvider _provider;
        private readonly IHubContext<UsersHub> _users;

        private static ConcurrentQueue<GamePoolItem> _collection;

        static GamePool()
        {
            _collection = new(new ConcurrentQueue<GamePoolItem>());
        }

        public GamePool(IServiceProvider provider)
        {
            _provider = provider;
            _users = _provider.GetService<IHubContext<UsersHub>>();
        }

        public async Task AddUserAsync(string userName, string userId)
        {
            Debug.WriteLine($"{userId}: {userName}");

            var gamePoolItem = new GamePoolItem(userId, userName);

            _collection.Enqueue(gamePoolItem);

            if (_collection.Count % 2 == 0)
            {
                if (_collection.TryDequeue(out var firstUser))
                {
                    if (_collection.TryDequeue(out var secondUser))
                    {
                        var randomValue = Random.Shared.Next(0, 2);

                        var first = firstUser;
                        var second = secondUser;

                        if (randomValue == 1)
                        {
                            (first, second) = (second, first);
                        }

                        var gameParameters = new GameStartParameters(first.UserId, second.UserId, first.UserName, second.UserName);

                        gameParameters.SenderUserName = second.UserName;
                        gameParameters.SenderId = second.UserId;

                        await _users.Clients.Users(first.UserId)
                            .SendAsync("OnStartMessage", gameParameters);

                        gameParameters.SenderUserName = first.UserName;
                        gameParameters.SenderId = first.UserId;


                        await _users.Clients.Users(second.UserId)
                            .SendAsync("OnStartMessage", gameParameters);
                    }
                }

            }
        }

        //public static IEnumerable<GamePoolItem> GetConsumingEnumerable() => _collection.GetConsumingEnumerable();
    }
}
