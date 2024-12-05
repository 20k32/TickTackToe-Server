using MongoDB.Bson;
using Shared.Api.Request;
using Shared.DTOs;

namespace Server.Persistence.Entity
{
    public static class UserExtensions
    {
        public static UserDto MapToDto(this User user) =>
            new(user.UserName, user.Password, user.PasswordSalt, user.Rating)
            {
                GameHistory = user.GameHistory.MapToGameHistoryDto()
            };

        public static User MapToUser(this UserDto user) =>
            new()
            {
                Id = ObjectId.Empty,
                UserName = user.UserName,
                Password = user.Password,
                PasswordSalt = user.PasswordSalt,
                Rating = user.Rating,
                GameHistory = user.GameHistory.MapToGameHistory(),
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

        public static ICollection<GameHistory> MapToGameHistory(this ICollection<GameHistoryDto> gameHistoryDto)
        {
            var result = new List<GameHistory>();

            foreach (var item in gameHistoryDto)
            {
                result.Add(new()
                {
                    OpponentName = item.OpponentName,
                    PointsReceived = item.PointsReceived,
                    TimeTaken = item.TimeTaken,
                });
            }

            return result;
        }

        public static ICollection<GameHistoryDto> MapToGameHistoryDto(this ICollection<GameHistory> gameHistoryDto)
        {
            var result = new List<GameHistoryDto>();

            foreach (var item in gameHistoryDto)
            {
                result.Add(new(item.TimeTaken, item.OpponentName, item.PointsReceived));
            }

            return result;
        }
    }
}
