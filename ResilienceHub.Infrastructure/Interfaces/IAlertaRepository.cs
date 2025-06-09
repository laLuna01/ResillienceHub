using ResilienceHub.Core.Models;

namespace ResilienceHub.Infrastructure.Interfaces;

public interface IAlertaRepository
{
    Task<IEnumerable<Alerta>> GetAlertas();
    Task<Alerta> GetAlerta(int id);
    Task<IEnumerable<Alerta>> GetAlertasAtivos();
    Task<IEnumerable<Alerta>> GetAlertasPorTipo(string tipo);
    Task<IEnumerable<Alerta>> GetAlertasPorSeveridade(string severidade);
    Task<IEnumerable<Alerta>> GetAlertasPorStatus(string status);
    Task<Alerta> CreateAlerta(Alerta alerta);
    Task UpdateAlerta(Alerta alerta);
    Task UpdateAlertaStatus(int id, string status);
    Task DeleteAlerta(int id);
}