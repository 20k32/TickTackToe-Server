
using Server.Controllers;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup();

            startup.ConfigureServices(builder.Services, builder.Configuration);


            var app = builder.Build();
            startup.Configure(app, app.Environment);
            
            app.MapControllers();
            app.MapHub<UsersHub>("/users");

            app.Run();
        }
    }
}
