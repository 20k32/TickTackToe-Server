using MongoDB.Driver;
using Server.Core.Auth;
using Server.Core.Services;
using Server.Persistence.MongoDb;

namespace Server.Core
{
    public static class DiExtensions
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<TokenGenerator>();

            return serviceCollection;
        }


        public static IServiceCollection RegiseterDatabaseServices(this IServiceCollection serviceCollection, ConfigurationManager manager)
        {
            var mongoClient = new MongoClient(manager["MongoDbConnectionString"]);
            serviceCollection.AddSingleton<IMongoClient>(mongoClient);

            var userRepository = new UserRepository(mongoClient);
            _ = userRepository.ConnectToDbAsync(manager["MongoDbUserDatabaseName"]);

            serviceCollection.AddSingleton(userRepository);
            serviceCollection.AddSingleton<UserService>();

            return serviceCollection;
        }
    }
}
