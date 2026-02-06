using Orders.Kafka.Contracts;
using Application.Extensions;
using Infrastructure.Configuration.Extensions;
using Infrastructure.Configuration.HostedServices;
using Infrastructure.Kafka.Extensions;
using Infrastructure.Kafka.Handlers;
using Infrastructure.Postgres.Extensions;
using Infrastructure.Postgres.HostedServices;
using Presentation.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomConfiguration(builder.Configuration)
    .AddManualConfigurationClient();

builder.Services
    .AddDataSource(builder.Configuration)
    .AddFluentMigrations(builder.Configuration)
    .AddRepositories();

builder.Services
    .AddApplicationServices();

builder.Services
    .AddMessaging()
    .AddMessageSubscriber<OrderProcessingValue, OrderProcessingHandler>("OrderProcessing");

builder.Services
    .AddPresentationServices(builder.Configuration);

builder.Services.AddHostedService<ConfigurationInitializationService>();
builder.Services.AddHostedService<MigrationHostedService>();
builder.Services.AddHostedService<BackgroundConfigurationService>();

WebApplication app = builder.Build();

app.MapGrpcEndpoints();
app.Run();
