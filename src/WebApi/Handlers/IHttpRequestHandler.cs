using MediatR;
using WebApi.Requests;

namespace WebApi.Handlers;

public interface IHttpRequestHandler<in TRequest> : IRequestHandler<TRequest, IResult> where TRequest : IHttpRequest
{
    
}