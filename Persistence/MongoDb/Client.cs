using MongoDB.Driver;

namespace Server.Persistence.MongoDb
{
    public class Client
    {
        public static IMongoClient Instance { get; private set; }

        public void Connect(string connectionString)
        {
            Instance = new MongoClient(connectionString);
        }
    }
}
