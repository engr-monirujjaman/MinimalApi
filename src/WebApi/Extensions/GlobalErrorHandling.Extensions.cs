using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace WebApi.Extensions;

public static class GlobalExceptionHandlerExtensions
{
    public static void ConfigureGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(configure =>
        {
            configure.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature is not null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        ValidationException => StatusCodes.Status400BadRequest,
                        _ => (int) HttpStatusCode.InternalServerError
                    };

                    Log.Error("Something went wrong: {ContextFeatureError}", contextFeature.Error.Message);

                    await context.Response.WriteAsJsonAsync( contextFeature.Error switch
                    {
                        UnauthorizedAccessException => Results.Fail(StatusCodes.Status401Unauthorized, new [] {"Unauthorized User, Please Try Again Later After Login."}),
                        ValidationException exception => Results.Fail(exception.Errors),
                        _ => Results.Fail(StatusCodes.Status500InternalServerError, new [] {"Internal Server Error, Please Try Again Later."})
                    });
                }
            });
        });
    }

    // private static IResult PrepareResponse(IExceptionHandlerFeature exceptionFeature) => exceptionFeature.Error switch
    // {
    //     UnauthorizedAccessException => Results.Unauthorized(),
    //     _ => Results.BadRequest()
    // };
    //
    // private static IEnumerable<object> GetErrorMessages(IEnumerable<ValidationFailure> validationFailures)
    // {
    //     var a = validationFailures.GroupBy(x => x.PropertyName)
    //         .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage));
    //
    //     return ;
    // }
}

