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

            v1Group.MapFilialEndpoints();
            v1Group.MapMotoEndpoints();
            v1Group.MapFuncionarioEndpoints();

            // Mantido para uso na Sprint 4
            var v2Group = app.MapGroup("/api/v2")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(2, 0)
                .WithTags("Management Endpoints V2");
        }
    }