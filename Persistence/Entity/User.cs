using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Persistence.Entity
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public double Rating { get; set; }
        public ICollection<GameHistory> GameHistory { get; set; }
    }
}
