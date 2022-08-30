using WebApi.Requests;

namespace WebApi.Handlers;

public class SecretHandler : IHttpRequestHandler<SecretRequest>
{
    public Task<IResult> Handle(SecretRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Success(new
        {
            Message = $"Your FullName: {request.FirstName} {request.LastName}."
        }));
    }
}