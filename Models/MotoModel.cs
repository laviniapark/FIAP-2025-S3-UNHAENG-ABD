namespace ManagementApp.Models ;

    [Serializable]
    public record MotoRequest(
        string Placa, 
        string Marca,
        string Modelo, 
        int Ano, 
        Moto.StatusEnum Status, 
        Guid FilialId
        );

    public record MotoResponse(
        Guid Motoid, 
        string Placa,
        string Marca,
        string Modelo, 
        int Ano, 
        Moto.StatusEnum Status, 
        Guid FilialId,
        string FilialNome,
        List<LinkModel> Links
        );
        
    public record MotoResponseGA(
        Guid Motoid, 
        string Placa,
        string Marca,
        string Modelo, 
        int Ano, 
        Moto.StatusEnum Status, 
        Guid FilialId,
        string FilialNome
        );