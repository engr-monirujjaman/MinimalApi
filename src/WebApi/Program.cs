using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WebApi.Data;
using WebApi.Endpoints;
using WebApi.Extensions;
using WebApi.PipelineBehaviors;
using WebApi.Requests;
using JwtConstants = WebApi.Settings.JwtConstants;

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

// Identity DbContext Config Start

builder.Services.AddDbContext<AppDbContext>(config => { config.UseInMemoryDatabase("IdentityServerMemory"); });

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequireDigit = false;
        config.Password.RequiredLength = 4;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
    }).AddUserManager<UserManager<IdentityUser>>()
    .AddSignInManager<SignInManager<IdentityUser>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(JwtConstants.Key);
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtConstants.Issuer,
            ValidAudience = JwtConstants.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.Append("my-custom-header", "custom-value");
                await context.Response.WriteAsync(Results
                    .Fail(StatusCodes.Status401Unauthorized, new[] {"You are not authorized"}).ToJson());
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

// Identity DbContext Config End

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

app.UseSwagger(options => { options.RouteTemplate = "swagger/{documentName}/swagger.json"; });

app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Minimal Api V1"));

// Add Swagger Middlewares End

// Config Global Exception Handler Start

app.ConfigureGlobalExceptionHandler();

// Config Global Exception Handler End

// app.UseSerilogRequestLogging();

// app.Use(async (context, next) =>
// {
//     await next();
//     
//     if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
//     {
//         context.Response.ContentType = "application/json";
//         await context.Response.WriteAsync(new
//         {
//             StatusCode = 401,
//             Message = "Token is not valid"
//         }.ToJson());
//     }
// });

app.UseAuthentication();
app.UseAuthorization();

app.MediateGet<ExampleRequest>("example/{name}");
app.MediateGet<SecretRequest>("secret/fullName");
app.MapCustomerEndpoints();

app.Run();