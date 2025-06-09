using ResilienceHub.Core.Models;

namespace ResilienceHub.Infrastructure.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetUsuarios();
    Task<Usuario> GetUsuario(int id);
    Task<Usuario> GetUsuarioByCpf(string cpf);
    Task<Usuario> GetUsuarioByEmail(string email);
    Task<IEnumerable<Usuario>> GetUsuariosPorTipo(string tipo);
    Task<Usuario> CreateUsuario(Usuario usuario);
    Task UpdateUsuario(Usuario usuario);
    Task UpdateUsuarioToken(int id, string token);
    Task DeleteUsuario(int id);
}