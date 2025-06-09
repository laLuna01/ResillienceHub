using System.ComponentModel.DataAnnotations;

namespace ResilienceHub.Core.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
        public string Tipo { get; set; } // admin, gerente, voluntario, afetado

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(100, ErrorMessage = "O email não pode exceder 100 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        public string Senha { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres.")]
        public string Telefone { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? DataUltimaLocalizacao { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public string TokenNotificacao { get; set; }
    }

    public class UsuarioDTO : LinkResourceBase
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Tipo { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? DataUltimaLocalizacao { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}

