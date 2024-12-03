using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Core.GamePool;
using Server.Models.Extensions;
using Shared.Api.Models;
using Shared.Api.Result;

namespace Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private GamePool _gamePool;

        public GameController(IServiceProvider provider)
        {
            _gamePool = provider.GetService<GamePool>();
        }

        [HttpGet("pool")]
        public async IAsyncEnumerable<UserItemPool> GetPool()
        {
            await foreach(var item in _gamePool.GetAsyncEnumerable())
            {
                yield return new(item.UserName);
            }
        }

        [HttpPut("pool/{userName}")]
        public async Task<IActionResult> AddMeToPool(string userName)
        {
            PlainResult result;

            var id = Request.TryGetUserId();

            if(id is null)
            {
                result = new ("Your id is empty", StatusCodes.Status401Unauthorized);
            }

            try
            {
                await _gamePool.AddUserAsync(userName, id);
                result = new ("Successfully added you to pool", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                result = new ($"Internal server error while adding you to pool.\n{ex.Message}\n{ex.StackTrace}",
                    StatusCodes.Status500InternalServerError);
            }

            return result.ToObjectResult();
        }

        [HttpDelete("pool")]
        public IActionResult RemoveMeFromPool()
        {
            PlainResult result;

            var id = Request.TryGetUserId();

            if (id is null)
            {
                result = new PlainResult("Your id is empty", StatusCodes.Status401Unauthorized);
            }

            try
            {
                _gamePool.RemoveUser(id);

                result = new PlainResult("Successfully removed you from pool", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                result = new($"Internal server error while removing you from pool.\n{ex.Message}\n{ex.StackTrace}",
                    StatusCodes.Status500InternalServerError);
            }

            return result.ToObjectResult();
        }

    }
}
