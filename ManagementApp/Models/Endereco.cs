using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementApp.Models ;

    public class Endereco
    {
        public string CEP { get; set; }
        
        public string Logradouro { get; set; }
        
        public string Numero { get; set; }
        
        public string? Complemento { get; set; }
        
        public string Bairro { get; set; }
        
        public string Cidade { get; set; }
        
        public string UF { get; set; }
        
        public string Pais { get; set; }
    }