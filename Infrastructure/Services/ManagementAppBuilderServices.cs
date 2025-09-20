using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Asp.Versioning;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ManagementApp.Infrastructure.Services ;

    public static class ManagementAppBuilderServices
    {
        public static void RegisterManagementAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Database

            services.AddDbContext<ManagementDb>(options => 
                options.UseOracle(configuration.GetConnectionString("DefaultConnection")));
            
            #endregion
            
            #region JsonContent

            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.SerializerOptions.WriteIndented = true;
                
                options.SerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
                    );
            });

            #endregion

            #region HealthCheck

            services.AddHealthChecks()
                .AddOracle(
                    connectionString: configuration.GetConnectionString("DefaultConnection"),
                    name: "oracle-database-fiap",
                    healthQuery:"SELECT 1 FROM DUAL",
                    failureStatus:HealthStatus.Degraded,
                    timeout:TimeSpan.FromSeconds(10),
                    tags:new[]{"oracle","database"}
                    );

            #endregion

            #region RateLimit

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter(policyName: "fixed", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromSeconds(60);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
            });

            #endregion
            
            #region Scalar/Swagger/OpenApi

            services.AddOpenApi();

            #endregion

            #region Versioning

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
            });

            #endregion

            #region Idempotency

            services.AddDistributedMemoryCache();
            services.AddIdempotentMinimalAPI(new IdempotencyOptions
            {
                DistributedCacheKeysPrefix = "ManagementApp_",
                ExpiresInMilliseconds = 3600000,
                HeaderKeyName = "Idempotency-Key",
                IsIdempotencyOptional = false
            });
            services.AddIdempotentAPIUsingDistributedCache();

            #endregion
        }
    }