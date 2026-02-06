using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Presentation.Api.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (InvalidCastException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (ArgumentException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (OperationCanceledException exception)
        {
            throw new RpcException(new Status(StatusCode.Cancelled, exception.Message));
        }
    }
}
