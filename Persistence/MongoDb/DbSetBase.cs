using MongoDB.Bson;
using MongoDB.Driver;

namespace Server.Persistence.MongoDb
{
    public abstract class DbSetBase
    {
        protected IMongoClient Client;
        protected IMongoDatabase Database;

        public DbSetBase(IMongoClient client)
        {
            Client = client;
        }

        public abstract Task ConnectToDbAsync(string databaseName);
        public abstract void DisconnectFromDb();

        protected async Task<IMongoCollection<TEntity>> GetEntityByNameAsync<TEntity>(string entityName)
        {
            IMongoCollection<TEntity> result = null!;

            var collectionNamesCursor = await Database.ListCollectionNamesAsync(
                    new ListCollectionNamesOptions()
                    {
                        Filter = Builders<BsonDocument>.Filter.Eq("name", entityName)
                    });

            IMongoCollection<TEntity> entity = null;

            using (collectionNamesCursor)
            {
                var name = await collectionNamesCursor
                    .ToAsyncEnumerable()
                    .FirstOrDefaultAsync();

                if (name is not null)
                {
                    entity = Database.GetCollection<TEntity>(name);
                }
            }

            if (entity is null)
            {
                result = null;
            }

            return result;
        }

        public void Dispose()
        {
            DisconnectFromDb();
        }
    }
}
