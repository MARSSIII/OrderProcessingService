using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Application.Abstractions.Repositories;
using Infrastructure.Postgres.HostedServices;
using Infrastructure.Postgres.Migrations;
using Infrastructure.Postgres.Options;
using Infrastructure.Postgres.Plugins;
using Infrastructure.Postgres.Repositories;

namespace Infrastructure.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionOptions>(configuration.GetSection("Postgres"));

        return services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider =>
                {
                    IOptions<ConnectionOptions> options = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>();

                    return options.Value.BuildConnectionString();
                })
                .WithMigrationsIn(typeof(InitialMigration).Assembly));
    }

    public static IServiceCollection AddDataSource(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionOptions>(configuration.GetSection("Postgres"));

        return services.AddSingleton(serviceProvider =>
        {
            IOptions<ConnectionOptions> options = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(options.Value.BuildConnectionString());
            var mappingPlugin = new MappingPlugin();

            mappingPlugin.Configure(dataSourceBuilder);

            return dataSourceBuilder.Build();
        });
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IOrderItemRepository, OrderItemRepository>()
            .AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
    }

    public static IServiceCollection AddHostedMigrationServices(
        this IServiceCollection services)
    {
        services.AddHostedService<MigrationHostedService>();

        return services;
    }
}
