using Microsoft.AspNetCore.Builder;
using Presentation.Api.Controllers;

namespace Presentation.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapGrpcEndpoints(this WebApplication app)
    {
        app.MapGrpcService<ProductController>();
        app.MapGrpcService<OrderController>();

        return app;
    }
}
