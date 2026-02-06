using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Gateway.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Order Gateway API",
                Description = "HTTP Gateway for Order and Product gRPC services",
            });

            options.EnableAnnotations();
        });

        return services;
    }
}