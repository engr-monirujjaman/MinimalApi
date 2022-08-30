using WebApi.Dtos;
using WebApi.Requests;

namespace WebApi.Handlers;

public class ExampleHandler : IHttpRequestHandler<ExampleRequest>
{
    public Task<IResult> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Success(new ExampleResponseDto
        {
            Message = $"The age was: {request.Age} and the name was: {request.Name}"
        }));
    }
}