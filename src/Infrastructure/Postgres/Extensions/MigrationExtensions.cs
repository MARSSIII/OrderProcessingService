using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres.Extensions;

public static class MigrationExtensions
{
    public static void RunMigrations(this IServiceProvider serviceProvider)
    {
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateUp();
    }

    public static void RollbackMigrations(this IServiceProvider serviceProvider, long version)
    {
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateDown(version);
    }
}
