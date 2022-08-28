﻿using System.Reflection;
using MediatR;

namespace WebApi.PipelineBehaviors;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Handling {Name}", typeof(TRequest).Name);

        var myType = request.GetType();

        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

        foreach (var prop in props)
        {
            var propValue = prop.GetValue(request, null)!;
            _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        }

        var response = await next();

        _logger.LogInformation("Handled {Name}", typeof(TResponse).Name);
        
        return response;
    }
}