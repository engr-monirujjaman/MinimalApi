using MediatR;
using WebApi.Requests;

namespace WebApi.Handlers;

public class ExampleHandler : IRequestHandler<ExampleRequest, IResult>
{
    public Task<IResult> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        // throw new InvalidOperationException("Invalid operation exception from example handler");
        
        return Task.FromResult(Results.Ok(new
        {
            message = $"The age was: {request.Age} and the name was: {request.Name}"
        }));
    }
}