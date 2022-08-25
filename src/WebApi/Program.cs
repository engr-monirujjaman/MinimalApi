using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WebApi.Extensions;

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

// Swagger Services Config Start

// Configure Global Exception Filter 
// TODO committed because use minimal api
// builder.Services.AddControllers(configure => configure.Filters.Add(typeof(GlobalExceptionFilterAttribute)));

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

// Config Global Exception Handler

app.ConfigureGlobalExceptionHandler(Log.Logger);

app.MapGet("/", () => "Hello World!");

app.Run();