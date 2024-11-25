using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Server.Core.Services;
using Server.Models.Exceptions;
using Server.Models.Extensions;
using Server.Persistence.Entity;
using Shared.Api.Result;
using Shared.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(IServiceProvider provider)
        {
            _userService = provider.GetService<UserService>();
        }

        /// <summary>
        /// Updates user fields except password because it requires additional logic.
        /// </summary>
        [ProducesResponseType(typeof(UserResult), 200)]
        [HttpPut("my")]
        [Authorize]
        public async Task<IResult> UpdateUserById([FromBody] UserDto user)
        {
            UserResult result;
            var id = Request.TryGetUserId();
            ObjectId userId;

            if (id is null)
            {
                result = new UserResult(null, "Missing id.", StatusCodes.Status400BadRequest);
            }
            if (!ObjectId.TryParse(id, out userId))
            {
                result = new(null, "Unable get id from jwt.",
                    StatusCodes.Status401Unauthorized);
            }
            if (user is null)
            {
                result = new UserResult(null, "Missing body.", StatusCodes.Status400BadRequest);
            }
            else if (user.IsNull())
            {
                result = new UserResult(null, "Missing parameters in body.", StatusCodes.Status400BadRequest);
            }
            else
            {
                try
                {
                    var userResult = await _userService.UpdateUser(userId, user);

                    result = new(userResult, "There is such user in database.",
                        StatusCodes.Status200OK);
                }
                catch (NotFoundUserInDbException ex)
                {
                    result = new(null, ex.Message, StatusCodes.Status403Forbidden);
                }
                catch (Exception ex)
                {
                    result = new(null, $"Internal server error while updating user.\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return Results.Json(result);
        }

        /// <summary>
        /// Checks for authorized user and returns, parses jwt and return info if he exists.
        /// </summary>
        [Authorize]
        [HttpGet("my")]
        [ProducesResponseType(typeof(UserResult), 200)]
        public async Task<IResult> GetUserFromJwt()
        {
            ObjectId userId;

            UserResult result;
            var id = Request.TryGetUserId();

            if (id is null)
            {
                result = new(null, "Missing id.",
                    StatusCodes.Status401Unauthorized);
            }
            else if (!ObjectId.TryParse(id, out userId))
            {
                result = new(null, "Unable get id from jwt.",
                    StatusCodes.Status401Unauthorized);
            }
            else
            {
                try
                {
                    var objectId = ObjectId.Parse(id);

                    var userDto = await _userService.GetUserByIdAsync(objectId);

                    result = new(userDto, "There is such user in database",
                        StatusCodes.Status200OK);
                }
                catch (NotFoundUserInDbException ex)
                {
                    result = new(null, ex.Message,
                        StatusCodes.Status404NotFound);
                }
                catch (Exception ex)
                {
                    result = new(null, $"Internal server error while getting user\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return Results.Json(result);
        }
    }
}
