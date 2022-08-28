using MediatR;
using MediatR.Pipeline;

namespace WebApi.PipelineBehaviors;

public class RequestPostProcessorBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        Log.Information("Handled {Name}", typeof(TResponse).Name);
        return Task.CompletedTask;
    }
}