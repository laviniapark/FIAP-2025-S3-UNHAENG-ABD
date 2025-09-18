namespace UnhaengABD.Models ;

    public class Moto
    {
        public Guid MotoId { get; set; } = Guid.NewGuid();
        
        public required string Placa { get; set; }
        
        public required string Modelo  { get; set; }
        
        public required int Ano  { get; set; }

        public StatusEnum Status { get; set; }
        
        public Guid FilialId { get; set; }
        public Filial Filial { get; set; }

        public enum StatusEnum
        {
            Disponivel,
            Manutencao,
            Ocupada
        }
    }