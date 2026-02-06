using Gateway.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGrpcExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GrpcExceptionMiddleware>();
    }
}