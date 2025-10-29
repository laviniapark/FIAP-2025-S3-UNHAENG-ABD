using Scalar.AspNetCore;

namespace ManagementApp.Infrastructure.Middlewares ;

    public static class ManagementAppMiddlewares
    {
        public static void UseManagementAppMiddlewares(this WebApplication app, WebApplicationBuilder builder)
        {
            #region ApiKeys

            var apiKeys = builder.Configuration.GetSection("ApiKeys").Get<List<string>>();

            app.Use(async (ctx, next) =>
            {
                var endpoint = ctx.GetEndpoint();
                var allowAnonymous = endpoint?.Metadata.GetMetadata<
                    Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;

                if (allowAnonymous)
                {
                    await next();
                    return;
                }

                if (endpoint?.DisplayName.Contains("Management Endpoints V2") == true)
                {
                    if (apiKeys != null && (
                        !ctx.Request.Headers.TryGetValue("X-API-Key", out var provided) ||
                        !apiKeys.Contains(provided)))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await ctx.Response.WriteAsJsonAsync("API Key is missing or invalid.");
                        return;
                    }
                }
                
                await next();
            });

            #endregion

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseRateLimiter();
        }
    }