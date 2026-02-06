using Gateway.Models.Responses;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Gateway.Middlewares;

public class GrpcExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex)
        {
            await HandleRpcExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static async Task HandleRpcExceptionAsync(HttpContext context, RpcException exception)
    {
        (HttpStatusCode statusCode, string message) = MapGrpcStatusToHttp(exception.StatusCode, exception.Status.Detail);

        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponse(
            Message: message,
            Details: exception.Status.Detail);

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new ErrorResponse(
            Message: "Internal server error",
            Details: exception.Message);

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static (HttpStatusCode StatusCode, string Message) MapGrpcStatusToHttp(StatusCode grpcStatusCode, string detail)
    {
        return grpcStatusCode switch
        {
            StatusCode.OK => (HttpStatusCode.OK, "Success"),
            StatusCode.Cancelled => (HttpStatusCode.RequestTimeout, "Request was cancelled"),
            StatusCode.Unknown => (HttpStatusCode.InternalServerError, "Unknown error occurred"),
            StatusCode.InvalidArgument => (HttpStatusCode.BadRequest, detail),
            StatusCode.DeadlineExceeded => (HttpStatusCode.GatewayTimeout, "Request deadline exceeded"),
            StatusCode.NotFound => (HttpStatusCode.NotFound, detail),
            StatusCode.AlreadyExists => (HttpStatusCode.Conflict, detail),
            StatusCode.PermissionDenied => (HttpStatusCode.Forbidden, "Permission denied"),
            StatusCode.ResourceExhausted => (HttpStatusCode.TooManyRequests, "Resource exhausted"),
            StatusCode.FailedPrecondition => (HttpStatusCode.PreconditionFailed, detail),
            StatusCode.Aborted => (HttpStatusCode.Conflict, "Operation was aborted"),
            StatusCode.OutOfRange => (HttpStatusCode.BadRequest, "Argument out of range"),
            StatusCode.Unimplemented => (HttpStatusCode.NotImplemented, "Operation not implemented"),
            StatusCode.Internal => (HttpStatusCode.InternalServerError, "Internal server error"),
            StatusCode.Unavailable => (HttpStatusCode.ServiceUnavailable, "Service unavailable"),
            StatusCode.DataLoss => (HttpStatusCode.InternalServerError, "Data loss occurred"),
            StatusCode.Unauthenticated => (HttpStatusCode.Unauthorized, "Authentication required"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred"),
        };
    }
}