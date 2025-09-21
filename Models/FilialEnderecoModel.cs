namespace ManagementApp.Models ;

    [Serializable]
    public record EnderecoRequest(
        string CEP,
        string Logradouro,
        string Numero,
        string? Complemento,
        string Bairro,
        string Cidade,
        string UF,
        string Pais
        );

    public record FilialRequest(
        string Nome, 
        string Cnpj, 
        string? Telefone, 
        DateTime? DataAbertura, 
        DateTime? DataEncerramento,
        EnderecoRequest Endereco
        );
        
    public record EnderecoResponse(
        string CEP,
        string Logradouro,
        string Numero,
        string? Complemento,
        string Bairro,
        string Cidade,
        string UF,
        string Pais
        );
        
    public record FilialResponse(
        Guid FilialId,
        string Nome, 
        string Cnpj, 
        string? Telefone, 
        DateTime? DataAbertura, 
        DateTime? DataEncerramento,
        EnderecoResponse Endereco,
        List<LinkModel> Links
        );
        
    public record FilialResponseGA(
        Guid FilialId,
        string Nome, 
        string Cnpj, 
        string? Telefone, 
        DateTime? DataAbertura, 
        DateTime? DataEncerramento,
        EnderecoResponse Endereco
        );
    