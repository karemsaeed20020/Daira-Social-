

namespace Daira.Api.Extensions
{
    public static class AppService
    {
        public static async Task ApplyMigrationWithSeed(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<DairaDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var env = services.GetRequiredService<IHostEnvironment>();
            try
            {
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error has occurred during migration or seeding!");
            }
        }
    }
}
