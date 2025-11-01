using Scalar.AspNetCore;

namespace ManagementApp.Infrastructure.Middlewares ;

    public static class ManagementAppMiddlewares
    {
        public static IApplicationBuilder UseManagementAppMiddlewares(this IApplicationBuilder app, IConfiguration config)
        {
            #region ApiKeys

            var apiKeys = config.GetSection("ApiKeys").Get<List<string>>();

            app.UseWhen(
                ctx => ctx.Request.Path.StartsWithSegments("/api/v2"),
                branch =>
                {
                    branch.Use(async (ctx, next) =>
                    {
                        var endpoint = ctx.GetEndpoint();
                        var allowAnonymous = endpoint?.Metadata
                            .GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;

                        if (allowAnonymous)
                        {
                            await next();
                            return;
                        }
                        
                        if (!ctx.Request.Headers.TryGetValue("X-API-Key", out var provided) ||
                            !apiKeys.Contains(provided))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await ctx.Response.WriteAsJsonAsync("API Key is missing or invalid.");
                            return;
                        }

                        await next();
                    });
                });

            #endregion

            app.UseRateLimiter();

            return app;
        }
    }