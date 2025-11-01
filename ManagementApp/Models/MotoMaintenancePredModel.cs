namespace ManagementApp.Models ;

    [Serializable]
    public record PredictResponse(
        float Quilometragem,
        float AnosUso,
        double CustoPrevisto
        );