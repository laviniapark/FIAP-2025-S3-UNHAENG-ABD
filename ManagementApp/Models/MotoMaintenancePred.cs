using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.ML.Data;

namespace ManagementApp.Models ;

    public class MotoData
    {
        public float Quilometragem { get; set; }
        public float AnosUso { get; set; }
        public float CustoManutencao { get; set; }
    }

    public class MaintenancePrediction
    {
        [ColumnName("Score")]
        public float CustoPrevisto { get; set; }
    }