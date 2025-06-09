using System.ComponentModel.DataAnnotations;

namespace ResilienceHub.Core.Models
{
    public class Abrigo
    {
        public int AbrigoId { get; set; }

        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        [StringLength(200, ErrorMessage = "A localização não pode exceder 200 caracteres.")]
        public string Localizacao { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero.")]
        public int Capacidade { get; set; }

        public int OcupacaoAtual { get; set; } = 0;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;
    }

    public class AbrigoDTO : LinkResourceBase
    {
        public int AbrigoId { get; set; }
        public string Nome { get; set; }
        public string Localizacao { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int Capacidade { get; set; }
        public int OcupacaoAtual { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }
}

