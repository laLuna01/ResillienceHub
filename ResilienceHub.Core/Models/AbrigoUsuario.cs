using System.ComponentModel.DataAnnotations;

namespace ResilienceHub.Core.Models;

public class AbrigoUsuario
{
    public int RegistroId { get; set; }

    [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
    public int AbrigoId { get; set; }

    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public int UsuarioId { get; set; }

    public DateTime DataCheckin { get; set; } = DateTime.Now;
    public DateTime? DataCheckout { get; set; }
    public string Status { get; set; } = "ativo"; // ativo, finalizado
}

public class AbrigoUsuarioDTO : LinkResourceBase
{
    public int RegistroId { get; set; }
    public int AbrigoId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataCheckin { get; set; }
    public DateTime? DataCheckout { get; set; }
    public string Status { get; set; }
    public string NomeAbrigo { get; set; }
    public string NomeUsuario { get; set; }
}

public class CheckInRequest
{
    [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
    public int AbrigoId { get; set; }

    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    public int UsuarioId { get; set; }
}

public class CheckOutRequest
{
    [Required(ErrorMessage = "O ID do registro é obrigatório.")]
    public int RegistroId { get; set; }
}