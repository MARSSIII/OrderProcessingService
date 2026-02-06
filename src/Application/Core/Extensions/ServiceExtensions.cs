using Microsoft.Extensions.DependencyInjection;
using Application.Abstractions.Messaging;
using Application.Contracts.Orders;
using Application.Contracts.Products;
using Application.Handlers;
using Application.Orders;
using Application.Products;

namespace Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderProcessingEventHandler, OrderProcessingEventHandler>();

        return services;
    }
}
