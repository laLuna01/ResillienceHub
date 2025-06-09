using System.ComponentModel.DataAnnotations;

namespace ResilienceHub.Core.Models
{
    public class Alerta
    {
        public int AlertaId { get; set; }

        [Required(ErrorMessage = "O tipo de alerta é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo não pode exceder 50 caracteres.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "A severidade é obrigatória.")]
        public string Severidade { get; set; } // baixa, media, alta, critica

        [Required(ErrorMessage = "A área geográfica é obrigatória.")]
        [StringLength(200, ErrorMessage = "A área geográfica não pode exceder 200 caracteres.")]
        public string AreaGeografica { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? RaioAfetado { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string Descricao { get; set; }

        public DateTime DataHoraInicio { get; set; } = DateTime.Now;
        public DateTime? DataEstimadaTermino { get; set; }

        [Required(ErrorMessage = "A fonte é obrigatória.")]
        [StringLength(100, ErrorMessage = "A fonte não pode exceder 100 caracteres.")]
        public string Fonte { get; set; }

        public string Status { get; set; } = "ativo"; // ativo, cancelado, finalizado
    }

    public class AlertaDTO : LinkResourceBase
    {
        public int AlertaId { get; set; }
        public string Tipo { get; set; }
        public string Severidade { get; set; }
        public string AreaGeografica { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? RaioAfetado { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime? DataEstimadaTermino { get; set; }
        public string Fonte { get; set; }
        public string Status { get; set; }
    }
}


