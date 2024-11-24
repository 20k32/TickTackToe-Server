using MongoDB.Bson;
using PrintMe.Server.Models.Api.ApiRequest;
using Shared.DTOs.UserDto;

namespace Server.Persistence.Entity
{
    public static class UserExtensions
    {
        public static UserDto MapToDto(this User user) =>
            new(user.UserName, user.Password, user.PasswordSalt, user.Rating);

        public static User MapToUser(this UserDto user) =>
            new()
            {
                Id = ObjectId.Empty,
                UserName = user.UserName,
                Password = user.Password,
                PasswordSalt = user.PasswordSalt,
                Rating = user.Rating
            };

        public static User MapToUser(this UserAuthRequest authRequest, string password, string salt) =>
            new()
            {
                Id = ObjectId.GenerateNewId(),
                UserName = authRequest.UserName,
                Password = password,
                PasswordSalt = salt,
                Rating = 0
            };
    }
}
