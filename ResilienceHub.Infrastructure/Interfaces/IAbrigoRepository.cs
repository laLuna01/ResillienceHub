using ResilienceHub.Core.Models;

namespace ResilienceHub.Infrastructure.Interfaces;

public interface IAbrigoRepository
{
    Task<IEnumerable<Abrigo>> GetAbrigos();
    Task<Abrigo> GetAbrigo(int id);
    Task<Abrigo> CreateAbrigo(Abrigo abrigo);
    Task UpdateAbrigo(Abrigo abrigo);
    Task DeleteAbrigo(int id);
}