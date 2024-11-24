using Shared.DTOs.UserDto;

namespace Server.Models.Api.Result
{
    public sealed class UserResult : ResultBase
    {
        public UserDto UserDto { get; init; }

        public UserResult(UserDto passwordUserDto, string message, int statusCode) : base(message, statusCode)
            => UserDto = passwordUserDto;
    }
}
