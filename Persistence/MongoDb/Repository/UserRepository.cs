using MongoDB.Bson;
using MongoDB.Driver;
using Server.Core;
using Server.Persistence.Entity;
using System.Xml.Linq;

namespace Server.Persistence.MongoDb
{
    public class UserRepository : DbSetBase
    {
        public IMongoCollection<User> Users;

        public UserRepository(IMongoClient client) : base(client)
        { }

        public override Task ConnectToDbAsync(string databaseName)
        {
            Database = Client.GetDatabase(databaseName);
            Users = Database.GetCollection<User>(CoreConstants.USER_COLLECTION_NAME);
            return Task.CompletedTask;
        }

        public Task AddAsync(User user) => Users.InsertOneAsync(user);

        public Task<User> RemoveByIdAsync(int id, User user)
        {
            var filter = Builders<User>.Filter.Eq(existing => existing.Id, user.Id);
            return Users.FindOneAndDeleteAsync(filter);
        }

        public async Task<User> UpadteByIdAsync(ObjectId id, User user)
        {
            var filter = Builders<User>.Filter.Eq(existing => existing.Id, id);

            var updateDefinition = Builders<User>.Update
                .Set(existing => existing.Rating, user.Rating);

            return await Users.FindOneAndUpdateAsync(filter, updateDefinition);
        }

        public async Task<User> GetByIdAsync(ObjectId id)
        {
            var filter = Builders<User>.Filter.Eq(existing => existing.Id, id);
            var cursor = await Users.FindAsync(filter);

            using (cursor)
            {
                var foundedEntries = cursor.ToAsyncEnumerable();

                var firstFounded = await foundedEntries.FirstOrDefaultAsync();

                return firstFounded;
            }
        }

        public async Task<User> GetByNameAsync(string name)
        {
            var filter = Builders<User>.Filter.Eq(existing => existing.UserName, name);

            var cursor = await Users.FindAsync<User>(filter);
            using (cursor)
            {
                var foundedEntries = cursor.ToAsyncEnumerable();

                var firstFounded = await foundedEntries.FirstOrDefaultAsync();

                return firstFounded;
            }
        }

        public override void DisconnectFromDb()
        {
            Database = null;
        }
    }
}
