using MediatR;
using MediatR.Pipeline;

namespace WebApi.PipelineBehaviors;

public class RequestPreProcessorBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : IRequest
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        Log.Information("Handling {Name}", typeof(TRequest).Name);
        return Task.CompletedTask;
    }
}