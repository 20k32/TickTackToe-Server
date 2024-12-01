using Microsoft.AspNetCore.Mvc;
using Server.Core.Services;
using Server.Models.Exceptions;
using Shared.Api.Request;
using Shared.Api.Result;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(IServiceProvider provider)
        {
            _userService = provider.GetService<UserService>();
        }

        /// <summary>
        /// Checks for user in database and generates token with fields: id, email, role.
        /// To view use https://jwt.io/
        /// </summary>
        [ProducesResponseType(typeof(TokenResult), 200)]
        [HttpPost("login")]
        public async Task<IResult> GenerateToken([FromBody] UserAuthRequest authRequest)
        {
            TokenResult result;

            if (authRequest is null)
            {
                result = new(null, null, "Missing body.",
                    StatusCodes.Status403Forbidden);
            }
            else if (authRequest.IsNull())
            {
                result = new(null, null, "Missing parameters in body.",
                    StatusCodes.Status403Forbidden);
            }
            else
            {
                try
                {
                    var tokenResult = await _userService.GenerateTokenAsync(authRequest);
                    result = new(tokenResult.token, tokenResult.userId, "Token successfully created.",
                        StatusCodes.Status200OK);
                }
                catch (NotFoundUserInDbException ex)
                {
                    result = new(null, null, ex.Message, StatusCodes.Status404NotFound);
                }
                catch (IncorrectPasswordException ex)
                {
                    result = new(null, null, ex.Message, StatusCodes.Status403Forbidden);
                }
                catch (Exception ex)
                {
                    result = new(null, null, $"Internal server error while generating token for user.\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return Results.Json(result);
        }

        /// <summary>
        /// Create a new user and save it to the database.
        /// </summary>
        [HttpPost("register")]
        public async Task<IResult> RegisterUser([FromBody] UserAuthRequest authRequest)
        {
            TokenResult result;

            if (authRequest is null)
            {
                result = new(null, null, "Missing body.",
                    StatusCodes.Status403Forbidden);
            }
            else if (authRequest.IsNull())
            {
                result = new(null, null, "Missing parameters in body.",
                    StatusCodes.Status403Forbidden);
            }
            else
            {
                try
                {
                    var tokenResult = await _userService.GenerateTokenForNewUserAsync(authRequest);
                    result = new(tokenResult.token, tokenResult.userId,"Token successfully created.",
                        StatusCodes.Status200OK);
                }
                catch(FoundUserInDbException ex)
                {
                    result = new(null, null, ex.Message,
                    StatusCodes.Status401Unauthorized);
                }
                catch (Exception ex)
                {
                    result = new(null, null, $"Internal server error while generating token for user.\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return Results.Json(result);
        }

    }
}
