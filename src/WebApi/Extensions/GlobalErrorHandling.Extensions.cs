using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using ILogger = Serilog.ILogger;

namespace WebApi.Extensions;

public static class GlobalExceptionHandlerExtensions
{
    public static void ConfigureGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(configure =>
        {
            configure.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                
                if(contextFeature is not null)
                { 
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        UnauthorizedAccessException => (int) HttpStatusCode.Unauthorized,
                        InvalidOperationException => (int) HttpStatusCode.BadRequest,
                        ValidationException => (int) HttpStatusCode.BadRequest,
                        _ => (int) HttpStatusCode.InternalServerError
                    };
                    
                    logger.Error("Something went wrong: {ContextFeatureError}", contextFeature.Error.Message);
                    
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                        new {
                        context.Response.StatusCode,
                        Message = "Internal Server Error."
                        }));
                }
            });
        });
    }
}