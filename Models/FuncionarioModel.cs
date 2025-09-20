namespace ManagementApp.Models ;

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
        string FilialNome
        );