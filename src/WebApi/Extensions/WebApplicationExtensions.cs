using MediatR;
using WebApi.Requests;

namespace WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder MediateGet<TRequest>(this WebApplication app, string template)
        where TRequest : IHttpRequest
    {
        return app.MapGet(template,
            async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    }

    public static RouteHandlerBuilder MediatePost<TRequest>(this WebApplication app, string template)
        where TRequest : IHttpRequest
    {
        return app.MapPost(template,
            async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    }


    public static RouteHandlerBuilder MediateDelete<TRequest>(this WebApplication app, string template)
        where TRequest : IHttpRequest
    {
        return app.MapDelete(template,
            async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    }


    public static RouteHandlerBuilder MediatePut<TRequest>(this WebApplication app, string template)
        where TRequest : IHttpRequest
    {
        return app.MapPut(template,
            async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    }
}