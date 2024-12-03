using Microsoft.AspNetCore.Mvc;
using Server.Core.Services;
using Server.Models.Exceptions;
using Server.Models.Extensions;
using Shared.Api.Models;
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
        [ProducesResponseType(typeof(ApiResult<TokenResult>), 200)]
        [HttpPost("login")]
        public async Task<IActionResult> GenerateToken([FromBody] UserAuthRequest authRequest)
        {
            PlainResult result;

            if (authRequest is null)
            {
                result = new("Missing body.",
                    StatusCodes.Status403Forbidden);
            }
            else if (authRequest.IsNull())
            {
                result = new("Missing parameters in body.",
                    StatusCodes.Status403Forbidden);
            }
            else
            {
                try
                {
                    var tokenResult = await _userService.GenerateTokenAsync(authRequest);
                    result = new ApiResult<TokenResult>(new(tokenResult.token, tokenResult.userId), "Token successfully created.",
                        StatusCodes.Status200OK);
                }
                catch (NotFoundUserInDbException ex)
                {
                    result = new(ex.Message, StatusCodes.Status404NotFound);
                }
                catch (IncorrectPasswordException ex)
                {
                    result = new(ex.Message, StatusCodes.Status403Forbidden);
                }
                catch (Exception ex)
                {
                    result = new($"Internal server error while generating token for user.\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return result.ToObjectResult();
        }

        /// <summary>
        /// Create a new user and save it to the database.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResult<TokenResult>), 200)]
        public async Task<IActionResult> RegisterUser([FromBody] UserAuthRequest authRequest)
        {
            PlainResult result;

            if (authRequest is null)
            {
                result = new("Missing body.",
                    StatusCodes.Status403Forbidden);
            }
            else if (authRequest.IsNull())
            {
                result = new("Missing parameters in body.",
                    StatusCodes.Status403Forbidden);
            }
            else
            {
                try
                {
                    var tokenResult = await _userService.GenerateTokenForNewUserAsync(authRequest);

                    result = new ApiResult<TokenResult>(new TokenResult(tokenResult.token, tokenResult.userId),"Token successfully created.",
                        StatusCodes.Status200OK);
                }
                catch(FoundUserInDbException ex)
                {
                    result = new(ex.Message,
                    StatusCodes.Status401Unauthorized);
                }
                catch (Exception ex)
                {
                    result = new($"Internal server error while generating token for user.\n{ex.Message}\n{ex.StackTrace}",
                        StatusCodes.Status500InternalServerError);
                }
            }

            return result.ToObjectResult();
        }

    }
}
