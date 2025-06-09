using System.ComponentModel.DataAnnotations;

namespace ResilienceHub.Core.Models
{
    public class Recurso
    {
        public int RecursoId { get; set; }

        [Required(ErrorMessage = "O tipo de recurso é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo não pode exceder 50 caracteres.")]
        public string Tipo { get; set; }

        [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres.")]
        public string Descricao { get; set; }

        public DateTime? Validade { get; set; }

        [StringLength(20, ErrorMessage = "A unidade de medida não pode exceder 20 caracteres.")]
        public string UnidadeMedida { get; set; } = "unidades";
    }

    public class RecursoDTO : LinkResourceBase
    {
        public int RecursoId { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public DateTime? Validade { get; set; }
        public string UnidadeMedida { get; set; }
    }
}

