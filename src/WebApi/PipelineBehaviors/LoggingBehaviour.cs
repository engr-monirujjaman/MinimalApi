using FluentValidation;
using MediatR;
using WebApi.Extensions;
using WebApi.Requests;


namespace WebApi.PipelineBehaviors;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IHttpRequest, IRequest<TResponse> where TResponse : IResult
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly HttpContext _httpContext;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        IEnumerable<IValidator<TRequest>> validators,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _validators = validators;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var requestUrl = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}{_httpContext.Request.PathBase}{_httpContext.Request.Path}{_httpContext.Request.QueryString}";
        
        _logger.LogInformation("Request Url: {Url}", requestUrl);
        
        _logger.LogInformation("Handling Request {NewLine}{Request}",Environment.NewLine , request.ToJson());

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();

        TResponse response;

        try
        {
            if (failures.Any())
            {
                _logger.LogWarning("Request Validation Failed: {NewLine}{@Errors}", Environment.NewLine, failures
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    ).ToJson());

                (_httpContext.Response.StatusCode, response) = (StatusCodes.Status400BadRequest,
                    (TResponse) Results.Fail(failures));

                return response;
            }

            response = await next();
            _httpContext.Response.StatusCode = response.StatusCode;
            _logger.LogInformation("Handled Response: {NewLine}{Response}", Environment.NewLine, response.ToJson());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);

            (_httpContext.Response.StatusCode, response) = e switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized,
                    (TResponse) Results.Fail(StatusCodes.Status401Unauthorized,
                        new[] {"Unauthorized User, Please Try Again Later After Login."})),
                _ => (StatusCodes.Status500InternalServerError,
                    (TResponse) Results.Fail(StatusCodes.Status500InternalServerError,
                        new[] {"Internal Server Error, Please Try Again Later."}))
            };
        }

        return response;
    }
}