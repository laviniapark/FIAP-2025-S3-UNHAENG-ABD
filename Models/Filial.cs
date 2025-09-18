namespace UnhaengABD.Models ;

    public class Filial
    {
        public Guid FilialId { get; set; } =  Guid.NewGuid();
        
        public required string Nome { get; set; }
        
        public required string Cnpj { get; set; }
        
        public string? Telefone { get; set; }
        
        public required DateTime DataAbertura { get; set; }
        
        public DateTime? DataEncerramento { get; set; }
        
        public required Endereco Endereco { get; set; }
        
        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
        public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
    }