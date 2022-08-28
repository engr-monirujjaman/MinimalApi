using MediatR;
using MediatR.Pipeline;

namespace WebApi.Extensions;

public class CommonExceptionHandler<TRequest, TResponse> : AsyncRequestExceptionHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<CommonExceptionHandler<TRequest, TResponse>> _logger;

    public CommonExceptionHandler(ILogger<CommonExceptionHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    protected override Task Handle(TRequest request, Exception exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        _logger.LogCritical("Exception Handing From Common");
        
        state.SetHandled(default!);

        return Task.CompletedTask;
    }
}

// public class ConnectionExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ConnectionException>
// {
//     private readonly TextWriter _writer;
//
//     public ConnectionExceptionHandler(TextWriter writer) => _writer = writer;
//
//     public async Task Handle(PingResource request,
//         ConnectionException exception,
//         RequestExceptionHandlerState<Pong> state,
//         CancellationToken cancellationToken)
//     {
//         // Exception type name must be written in messages by LogExceptionAction before
//         // Exception handler type name required because it is checked later in messages
//         await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ConnectionExceptionHandler).FullName}'").ConfigureAwait(false);
//         
//         state.SetHandled(new Pong());
//     }
// }
//
// public class AccessDeniedExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ForbiddenException>
// {
//     private readonly TextWriter _writer;
//
//     public AccessDeniedExceptionHandler(TextWriter writer) => _writer = writer;
//
//     public async Task Handle(PingResource request,
//         ForbiddenException exception,
//         RequestExceptionHandlerState<Pong> state,
//         CancellationToken cancellationToken)
//     {
//         // Exception type name must be written in messages by LogExceptionAction before
//         // Exception handler type name required because it is checked later in messages
//         await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(AccessDeniedExceptionHandler).FullName}'").ConfigureAwait(false);
//         
//         state.SetHandled(new Pong());
//     }
// }
//
// public class ServerExceptionHandler : IRequestExceptionHandler<PingNewResource, Pong, ServerException>
// {
//     private readonly TextWriter _writer;
//
//     public ServerExceptionHandler(TextWriter writer) => _writer = writer;
//
//     public virtual async Task Handle(PingNewResource request,
//         ServerException exception,
//         RequestExceptionHandlerState<Pong> state,
//         CancellationToken cancellationToken)
//     {
//         // Exception type name must be written in messages by LogExceptionAction before
//         // Exception handler type name required because it is checked later in messages
//         await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ServerExceptionHandler).FullName}'").ConfigureAwait(false);
//         
//         state.SetHandled(new Pong());
//     }
// }