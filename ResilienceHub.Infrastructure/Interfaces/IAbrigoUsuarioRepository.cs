using ResilienceHub.Core.Models;

namespace ResilienceHub.Infrastructure.Interfaces;

public interface IAbrigoUsuarioRepository
{
    Task<IEnumerable<AbrigoUsuario>> GetRegistros();
    Task<AbrigoUsuario> GetRegistro(int id);
    Task<IEnumerable<AbrigoUsuario>> GetRegistrosAtivosPorAbrigo(int abrigoId);
    Task<IEnumerable<AbrigoUsuario>> GetRegistrosPorUsuario(int usuarioId);
    Task<AbrigoUsuario> CreateCheckIn(AbrigoUsuario registro);
    Task<AbrigoUsuario> UpdateCheckOut(int registroId);
    Task DeleteRegistro(int id);
}