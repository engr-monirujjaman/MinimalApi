using MediatR;

namespace WebApi.Requests;

public interface IHttpRequest : IRequest<IResult>
{
    
}