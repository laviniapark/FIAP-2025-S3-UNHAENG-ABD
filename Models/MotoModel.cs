namespace ManagementApp.Models ;

    public record MotoRequest(
        string Placa, 
        string Modelo, 
        int Ano, 
        Moto.StatusEnum Status, 
        Guid FilialId
        );

    public record MotoResponse(
        Guid Motoid, 
        string Placa,  
        string Modelo, 
        int Ano, 
        Moto.StatusEnum Status, 
        string FilialNome
        );