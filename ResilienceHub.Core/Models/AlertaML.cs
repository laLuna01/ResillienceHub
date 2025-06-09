using Microsoft.ML.Data;

namespace ResilienceHub.Core.Models
{
    public class EventoInput
    {
        public string Tipo { get; set; }
        public string Local { get; set; }
        public float Magnitude { get; set; }
        public DateTime Tempo { get; set; }
        public int Populacao { get; set; }
    }

    public class EventoCsvData
    {
        [LoadColumn(0)]
        public string Tipo { get; set; }

        [LoadColumn(1)]
        public string Local { get; set; }

        [LoadColumn(2)]
        public float Magnitude { get; set; }

        [LoadColumn(3)]
        public DateTime Tempo { get; set; }

        [LoadColumn(4)]
        public int Populacao { get; set; }

        [LoadColumn(5)]
        public string Severidade { get; set; } // Label
    }

    public class EventoPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedSeveridade { get; set; }
    }
}


