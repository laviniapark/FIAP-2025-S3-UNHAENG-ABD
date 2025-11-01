using ManagementApp.Models;
using Microsoft.ML;

namespace ManagementApp.Endpoints ;

public static class PredictMaintenanceEndpoint
{
    public static RouteGroupBuilder MapPredictionEndpoint(this RouteGroupBuilder builder)
    {
        var v2Group = builder.MapGroup("/predict-manutencao")
            .WithTags("Prever Manutenção Endpoint");
        
        var mlContext = new MLContext();
        var data = new List<MotoData>
        {
            new MotoData { Quilometragem = 10000, AnosUso = 1, CustoManutencao = 500 },
            new MotoData { Quilometragem = 20000, AnosUso = 2, CustoManutencao = 1200 },
            new MotoData { Quilometragem = 30000, AnosUso = 3, CustoManutencao = 2000 },
            new MotoData { Quilometragem = 40000, AnosUso = 4, CustoManutencao = 2800 },
            new MotoData { Quilometragem = 50000, AnosUso = 5, CustoManutencao = 4000 }
        };
        var trainingData = mlContext.Data.LoadFromEnumerable(data);
        var pipeline = mlContext.Transforms.Concatenate("Features", "Quilometragem", "AnosUso")
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "CustoManutencao"));
        var model = pipeline.Fit(trainingData);
        var predEngine = mlContext.Model.CreatePredictionEngine<MotoData, MaintenancePrediction>(model);
        
        v2Group.MapPost("", (MotoData input) =>
        {
            var prediction = predEngine.Predict(input);
            return Results.Ok(new
            {
                input.Quilometragem,
                input.AnosUso,
                CustoPrevisto = Math.Round(prediction.CustoPrevisto, 2)
            });
        })
        .WithSummary("Prevê o custo de manutenção de uma moto")
        .WithDescription("Recebe dados como a quilometragem e os anos de uso de uma moto para calcular uma estimativa do custo de manutenção")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        return builder;
    }
}
