using Microsoft.AspNetCore.SignalR;
using Server.Controllers;
using Server.Core.Services;
using Server.Persistence.MongoDb;
using Shared.Api.Messages;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Server.Core.GamePool
{
    internal class GamePool
    {
        private readonly UserService _userService;
        private readonly IServiceProvider _provider;
        private readonly IHubContext<UsersHub> _users;

        private static ConcurrentDictionary<string, GamePoolItem> _collection;

        static GamePool()
        {
            _collection = new();
        }

        public GamePool(IServiceProvider provider)
        {
            _provider = provider;
            _users = _provider.GetService<IHubContext<UsersHub>>();
            _userService = _provider.GetService<UserService>();
        }

        public bool RemoveUser(string userId) => _collection.TryRemove(userId, out _);

        public async Task AddUserAsync(string userId)
        {

            var gamePoolItem = new GamePoolItem(userId);

            _collection.TryAdd(gamePoolItem.UserId, gamePoolItem);

            if (_collection.Count > 1)
            {
                var firstUser = _collection.Values.First();

                if (_collection.TryRemove(firstUser.UserId, out var _))
                {
                    var secondUser = _collection.Values.First();
                    
                    if (_collection.TryRemove(secondUser.UserId, out var _))
                    {
                        var randomValue = Random.Shared.Next(0, 2);

                        var first = firstUser;
                        var second = secondUser;

                        if (randomValue == 1)
                        {
                            (first, second) = (second, first);
                        }

                        var firstUserRaw = await _userService.GetUserByIdAsync(new(first.UserId));
                        var secondUserRaw = await _userService.GetUserByIdAsync(new(second.UserId));

                        var gameParameters = new GameStartParameters(first.UserId, second.UserId, firstUserRaw.UserName, secondUserRaw.UserName);

                        gameParameters.SenderUserName = secondUserRaw.UserName;
                        gameParameters.SenderId = second.UserId;

                        await _users.Clients.Users(first.UserId)
                            .SendAsync("OnStartMessage", gameParameters);

                        gameParameters.SenderUserName = firstUserRaw.UserName;
                        gameParameters.SenderId = first.UserId;


                        await _users.Clients.Users(second.UserId)
                            .SendAsync("OnStartMessage", gameParameters);
                    }
                }
            }
        }

        public IAsyncEnumerable<GamePoolItem> GetAsyncEnumerable() => _collection.Values.ToAsyncEnumerable();
    }
}
