namespace ManagementApp.Models ;

    public class Endereco
    {
        public Guid FilialId { get; set; }
        
        public required string CEP { get; set; }
        
        public required string Logradouro { get; set; }
        
        public required string Numero { get; set; }
        
        public string? Complemento { get; set; }
        
        public required string Bairro { get; set; }
        
        public required string Cidade { get; set; }
        
        public required string UF { get; set; }
        
        public required string Pais { get; set; }
        
        public required Filial Filial { get; set; }
    }