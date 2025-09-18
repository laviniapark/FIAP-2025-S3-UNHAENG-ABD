namespace UnhaengABD.Models ;

    public class Funcionario
    {
        public Guid FuncionarioId { get; set; } = Guid.NewGuid();
        
        public required string NomeCompleto { get; set; }
        
        public required string Cpf { get; set; }
        
        public required CargoEnum Cargo { get; set; }

        public bool? Ativo { get; set; } = true;
        
        public Guid FilialId { get; set; }
        public Filial Filial { get; set; }

        public enum CargoEnum
        {
            Funcionario, // funcionarios genericos
            Mecanico,
            Gestor,
            DonoFilial
        }
    }