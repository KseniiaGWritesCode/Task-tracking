using NLog;
using NLog.Web;
using TaskTracking;
using Microsoft.EntityFrameworkCore;

namespace TaskTracking
{
    // Entity framework

    // Controllers

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            logger.Info("Initialize app...");

            var builder = WebApplication.CreateBuilder(args);

            logger.Info($"Starting in Developer mode = {builder.Environment.IsDevelopment()}");


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add NLog
            //builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.Host.UseNLog();

            // Controllers
            builder.Services.AddControllers();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.MapControllers();


            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // Finish with EF. Add classes-models without DTO and stuff. Ensure that connected to DB and OLD ENTITIES ARE COMPATIBLE


            //app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
