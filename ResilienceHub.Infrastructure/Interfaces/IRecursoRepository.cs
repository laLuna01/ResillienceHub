using ResilienceHub.Core.Models;

namespace ResilienceHub.Infrastructure.Interfaces;

public interface IRecursoRepository
{
    Task<IEnumerable<Recurso>> GetRecursos();
    Task<Recurso> GetRecurso(int id);
    Task<IEnumerable<Recurso>> GetRecursosPorTipo(string tipo);
    Task<IEnumerable<Recurso>> GetRecursosVencidos();
    Task<IEnumerable<Recurso>> GetRecursosBaixoEstoque(int quantidadeMinima = 5);
    Task<Recurso> CreateRecurso(Recurso recurso);
    Task UpdateRecurso(Recurso recurso);
    Task AjustarQuantidadeRecurso(int id, int quantidade);
    Task DeleteRecurso(int id);
}