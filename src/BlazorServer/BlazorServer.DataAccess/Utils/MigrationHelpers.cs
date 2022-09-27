using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace BlazorServer.DataAccess.Utils;

public static class MigrationHelpers
{
    public static void MigrateDbContext<TContext>(
        this IServiceProvider serviceProvider,
        Action<IServiceProvider> postMigrationAction
    ) where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;
        var logger = scopedServiceProvider.GetRequiredService<ILogger<TContext>>();
        using var context = scopedServiceProvider.GetRequiredService<TContext>();

        logger.LogInformation("Migrating the DB associated with the context {Name}", typeof(TContext).Name);

        var retry = Policy.Handle<Exception>().WaitAndRetry(new[]
                                                            {
                                                                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10),
                                                                TimeSpan.FromSeconds(15),
                                                            });

        retry.Execute(() =>
                      {
                          context.Database.Migrate();
                          postMigrationAction(scopedServiceProvider);
                      });

        logger.LogInformation("Migrated the DB associated with the context {Name}", typeof(TContext).Name);
    }
}