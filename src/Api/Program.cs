using Gateway.Extensions;
using Infrastructure.Configuration.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomConfiguration(builder.Configuration)
    .AddSwagger()
    .AddGrpcClients()
    .AddGatewayServices()
    .AddControllers()
    .AddJsonOptions(
    options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

WebApplication app = builder.Build();

app.MapControllers();

app
    .UseGrpcExceptionHandling()
    .UseSwagger()
    .UseSwaggerUI();

app.Run();
