using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Exceptions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class GlobalExceptionFilterAttribute : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilterAttribute> _logger;

    public GlobalExceptionFilterAttribute(ILogger<GlobalExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled is false)
        {
            context.HttpContext.Response.StatusCode = context.Exception switch
            {
                UnauthorizedAccessException => (int) HttpStatusCode.Unauthorized,
                InvalidOperationException => (int) HttpStatusCode.BadRequest,
                ValidationException => (int) HttpStatusCode.BadRequest,
                _ => (int) HttpStatusCode.InternalServerError
            };

            context.HttpContext.Response.ContentType = "application/json";

            _logger.LogError("Something went wrong: Error in {ActionDescriptorDisplayName}.{NewLine1}{ExceptionMessage}.{NewLine2}Stack Trace: {ExceptionStackTrace}",
                context.ActionDescriptor.DisplayName, Environment.NewLine, context.Exception.Message, Environment.NewLine, context.Exception.StackTrace);

            context.Result = context.Exception switch
            {
                ValidationException exception => new JsonResult(exception.Errors),
                _ => new JsonResult(
                    new
                    {
                        error = new[] {context.Exception.Message},
                        stackTrace = context.Exception.StackTrace
                    })
            };
        }
    }
}