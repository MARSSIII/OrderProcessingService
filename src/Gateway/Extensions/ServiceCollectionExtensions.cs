using Gateway.Clients.GrpcOrderProcessing;
using Gateway.Clients.GrpcOrders;
using Gateway.Clients.GrpcProducts;
using Gateway.Mappers;
using Gateway.Middlewares;
using Gateway.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Presentation.Protos.Orders.V1;
using Presentation.Protos.Products.V1;
using GrpcContracts = Orders.ProcessingService.Contracts;

namespace Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services)
    {
        services.AddSingleton<OrderMapper>();
        services.AddSingleton<ProductMapper>();

        services.AddScoped<IOrderGrpcClient, OrderGrpcClient>();
        services.AddScoped<IProductGrpcClient, ProductGrpcClient>();
        services.AddScoped<IOrderProcessingGrpcClient, OrderProcessingGrpcClient>();

        services.AddScoped<GrpcExceptionMiddleware>();

        return services;
    }

    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddOptions<GrpcClientOptions>("Orders")
            .BindConfiguration("ClientOption");

        services.AddOptions<GrpcClientOptions>("Products")
            .BindConfiguration("ClientOption");

        services.AddOptions<GrpcClientOptions>("OrderProcessing")
            .BindConfiguration("OrderProcessingClient");

        services.AddGrpcClient<OrderGrpcService.OrderGrpcServiceClient>((serviceProvider, options) =>
        {
            GrpcClientOptions grpcOptions = serviceProvider.GetRequiredService<IOptionsMonitor<GrpcClientOptions>>().Get("Orders");

            options.Address = new Uri(grpcOptions.Address);
        });

        services.AddGrpcClient<ProductGrpcService.ProductGrpcServiceClient>((serviceProvider, options) =>
        {
            GrpcClientOptions grpcOptions = serviceProvider.GetRequiredService<IOptionsMonitor<GrpcClientOptions>>().Get("Products");

            options.Address = new Uri(grpcOptions.Address);
        });

        services.AddGrpcClient<GrpcContracts.OrderService.OrderServiceClient>((serviceProvider, options) =>
        {
            GrpcClientOptions grpcOptions = serviceProvider.GetRequiredService<IOptionsMonitor<GrpcClientOptions>>().Get("OrderProcessing");

            options.Address = new Uri(grpcOptions.Address);
        });

        return services;
    }
}
