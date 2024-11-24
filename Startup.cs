using Server.Core;
using Server.Core.Auth;

namespace Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services, ConfigurationManager manager)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.ConfigureAuthentication(manager)
                .AddEndpointsApiExplorer()
                .AddControllers();

            services.RegisterCoreServices()
                .RegiseterDatabaseServices(manager);
        }

        public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                builder.UseSwagger();
                builder.UseSwaggerUI();
            }
            builder.UseAuthentication();

            builder.UseRouting();

            builder.UseAuthorization();

            builder.UseCors(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    /*.AllowCredentials()*/
                    .AllowAnyOrigin()
                    .SetIsOriginAllowed(origin => true)
                    .WithExposedHeaders("Access-Control-Allow-Origin");
            });

            builder.UseHttpsRedirection();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });
        }
    }
}
