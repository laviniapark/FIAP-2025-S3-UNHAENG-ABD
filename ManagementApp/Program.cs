using Asp.Versioning;
using Asp.Versioning.Conventions;
using HealthChecks.UI.Client;
using ManagementApp.Endpoints;
using ManagementApp.Infrastructure.Middlewares;
using ManagementApp.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterManagementAppServices(builder.Configuration);

var app = builder.Build();

app.UseManagementAppMiddlewares(builder);

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersions(new List<ApiVersion> {
        new ApiVersion(1, 0),
        new ApiVersion(2, 0)
    })
    .Build();

app.RegisterManagementItemsEndpoints(apiVersionSet);

app.MapGet("/", () => "Hello World!")
    .WithName("Greetings")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1,0)
    .RequireRateLimiting("fixed");

app.MapHealthChecks("/api/v2/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
    .WithName("Health Check")
    .WithTags("Health Check Endpoint");

app.Run();

public partial class Program { }

