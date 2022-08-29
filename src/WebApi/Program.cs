using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WebApi.Extensions;
using WebApi.PipelineBehaviors;
using WebApi.Requests;

var builder = WebApplication.CreateBuilder(args);

// Config Serilog Start

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Verbose, theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);
builder.Host.UseSerilog(Log.Logger);
builder.Services.AddSingleton(Log.Logger);

// Config Serilog End

// Fluent Validation Config Start

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Fluent Validation Config End

// Mediator Config Start

builder.Services.AddMediatR(x => x.AsScoped(), typeof(Program));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
// builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
// builder.Services.AddTransient(typeof(AsyncRequestExceptionHandler<,>), typeof(CommonExceptionHandler<,>));
// builder.Services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(RequestPostProcessorBehaviour<,>));
// builder.Services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestPreProcessorBehaviour<>));

// Mediator Config End

// Swagger Services Config Start

// Configure Global Exception Filter 
// TODO committed because use minimal api
// builder.Services.AddControllers(configure => configure.Filters.Add(typeof(GlobalExceptionFilterAttribute)));

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Minimal Api", Version = "v1"});
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });
});

// Swagger Services Config End

var app = builder.Build();

// Add Swagger Middlewares Start

app.UseSwagger(options =>
{
    options.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Minimal Api V1"));

// Add Swagger Middlewares End

// Config Global Exception Handler Start

// app.ConfigureGlobalExceptionHandler();

// Config Global Exception Handler End

// app.UseSerilogRequestLogging();

app.MediateGet<ExampleRequest>("example/{name}");

app.Run();