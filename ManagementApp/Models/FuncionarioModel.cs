namespace ManagementApp.Models ;

    [Serializable]
    public record FuncionarioRequest(
        string NomeCompleto,
        string Cpf,
        Funcionario.CargoEnum Cargo,
        bool Ativo,
        Guid FilialId
        );
        
    public record FuncionarioResponse(
        Guid FuncionarioId,
        string NomeCompleto,
        string Cpf,
        Funcionario.CargoEnum Cargo,
        bool Ativo,
        Guid FilialId,
        string FilialNome,
        List<LinkModel> Links
        );
        
    public record FuncionarioResponseGA(
        Guid FuncionarioId,
        string NomeCompleto,
        string Cpf,
        Funcionario.CargoEnum Cargo,
        bool Ativo,
        Guid FilialId,
        string FilialNome
        );