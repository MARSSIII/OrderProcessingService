using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Api.Interceptors;

namespace Presentation.Api.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
        });

        return services;
    }
}
