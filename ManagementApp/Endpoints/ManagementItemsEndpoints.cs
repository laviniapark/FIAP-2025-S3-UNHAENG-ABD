using Asp.Versioning.Builder;

namespace ManagementApp.Endpoints ;

    public static class ManagementItemsEndpoints
    {
        public static void RegisterManagementItemsEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
        {

            var v1Group = app.MapGroup("/api/v1")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1, 0)
                .WithTags("Management Endpoints V1");

            v1Group.MapFilialEndpoints("V1");
            v1Group.MapMotoEndpoints("V1");
            v1Group.MapFuncionarioEndpoints("V1");

            // Mantido para uso na Sprint 4
            var v2Group = app.MapGroup("/api/v2")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(2, 0)
                .WithTags("Management Endpoints V2");

            v2Group.MapFilialEndpoints("V2");
            v2Group.MapMotoEndpoints("V2");
            v2Group.MapFuncionarioEndpoints("V2");
            v2Group.MapPredictionEndpoint();
        }
    }