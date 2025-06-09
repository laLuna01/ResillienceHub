using Microsoft.AspNetCore.Mvc;
using ResilienceHub.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.RateLimiting;
using ResilienceHub.Infrastructure.Interfaces;
using ResilienceHub.Infrastructure.Services;

namespace ResilienceHub.Api.Controllers
{
    /// <summary>
    /// Controlador para gerenciar operações relacionadas a alertas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaRepository _alertaRepository;
        private readonly MLService _mlService;

        /// <summary>
        /// Construtor do AlertasController.
        /// </summary>
        /// <param name="alertaRepository">Repositório de alerta.</param>
        public AlertaController(IAlertaRepository alertaRepository, MLService mlService)
        {
            _alertaRepository = alertaRepository;
            _mlService = mlService;
        }
        
        /// <summary>
        /// Busca todos os alertas no sistema.
        /// </summary>
        /// <returns>Retorna os alertas cadastrados.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertaDTO>>> GetAlertas()
        {
            var alertas = await _alertaRepository.GetAlertas();
            var alertasDTO = alertas.Select(a => new AlertaDTO
            {
                AlertaId = a.AlertaId,
                Tipo = a.Tipo,
                Severidade = a.Severidade,
                AreaGeografica = a.AreaGeografica,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                RaioAfetado = a.RaioAfetado,
                Descricao = a.Descricao,
                DataHoraInicio = a.DataHoraInicio,
                DataEstimadaTermino = a.DataEstimadaTermino,
                Fonte = a.Fonte,
                Status = a.Status
            }).ToList();

            foreach (var alerta in alertasDTO)
            {
                AddAlertaLinks(alerta);
            }

            return Ok(alertasDTO);
        }

        /// <summary>
        /// Obtém um alerta pelo seu ID.
        /// </summary>
        /// <param name="id">ID do alerta.</param>
        /// <returns>O alerta correspondente ou NotFound se não for encontrado.</returns>
        [HttpGet("{id}", Name = "GetAlerta")]
        public async Task<ActionResult<AlertaDTO>> GetAlerta(int id)
        {
            var alerta = await _alertaRepository.GetAlerta(id);

            if (alerta == null)
            {
                return NotFound();
            }

            var alertaDTO = new AlertaDTO
            {
                AlertaId = alerta.AlertaId,
                Tipo = alerta.Tipo,
                Severidade = alerta.Severidade,
                AreaGeografica = alerta.AreaGeografica,
                Latitude = alerta.Latitude,
                Longitude = alerta.Longitude,
                RaioAfetado = alerta.RaioAfetado,
                Descricao = alerta.Descricao,
                DataHoraInicio = alerta.DataHoraInicio,
                DataEstimadaTermino = alerta.DataEstimadaTermino,
                Fonte = alerta.Fonte,
                Status = alerta.Status
            };

            AddAlertaLinks(alertaDTO);

            return Ok(alertaDTO);
        }

        /// <summary>
        /// Insere um novo alerta no sistema.
        /// </summary>
        /// <param name="alerta">Dados do alerta a ser inserido.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpPost]
        public async Task<ActionResult<AlertaDTO>> PostAlerta([FromBody] Alerta alerta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAlerta = await _alertaRepository.CreateAlerta(alerta);

            var alertaDTO = new AlertaDTO
            {
                AlertaId = createdAlerta.AlertaId,
                Tipo = createdAlerta.Tipo,
                Severidade = createdAlerta.Severidade,
                AreaGeografica = createdAlerta.AreaGeografica,
                Latitude = createdAlerta.Latitude,
                Longitude = createdAlerta.Longitude,
                RaioAfetado = createdAlerta.RaioAfetado,
                Descricao = createdAlerta.Descricao,
                DataHoraInicio = createdAlerta.DataHoraInicio,
                DataEstimadaTermino = createdAlerta.DataEstimadaTermino,
                Fonte = createdAlerta.Fonte,
                Status = createdAlerta.Status
            };

            AddAlertaLinks(alertaDTO);

            return CreatedAtRoute("GetAlerta", new { id = alertaDTO.AlertaId }, alertaDTO);
        }

        /// <summary>
        /// Atualiza um alerta existente no sistema.
        /// </summary>
        /// <param name="id">ID do alerta a ser atualizado.</param>
        /// <param name="alerta">Novos dados do alerta.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpPut("{id}", Name = "UpdateAlerta")]
        public async Task<IActionResult> PutAlerta(int id, [FromBody] Alerta alerta)
        {
            if (id != alerta.AlertaId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _alertaRepository.UpdateAlerta(alerta);

            return NoContent();
        }

        /// <summary>
        /// Atualiza apenas o status de um alerta.
        /// </summary>
        /// <param name="id">ID do alerta.</param>
        /// <param name="status">Novo status.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpPatch("{id}/status", Name = "UpdateAlertaStatus")]
        public async Task<IActionResult> PatchAlertaStatus(int id, [FromBody] string status)
        {
            await _alertaRepository.UpdateAlertaStatus(id, status);
            return NoContent();
        }

        /// <summary>
        /// Exclui um alerta do sistema.
        /// </summary>
        /// <param name="id">ID do alerta a ser excluído.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpDelete("{id}", Name = "DeleteAlerta")]
        public async Task<IActionResult> DeleteAlerta(int id)
        {
            await _alertaRepository.DeleteAlerta(id);
            return NoContent();
        }

        /// <summary>
        /// Busca alertas ativos no sistema.
        /// </summary>
        /// <returns>Retorna os alertas ativos.</returns>
        [HttpGet("ativos", Name = "GetAlertasAtivos")]
        public async Task<ActionResult<IEnumerable<AlertaDTO>>> GetAlertasAtivos()
        {
            var alertas = await _alertaRepository.GetAlertasAtivos();
            var alertasDTO = alertas.Select(a => new AlertaDTO
            {
                AlertaId = a.AlertaId,
                Tipo = a.Tipo,
                Severidade = a.Severidade,
                AreaGeografica = a.AreaGeografica,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                RaioAfetado = a.RaioAfetado,
                Descricao = a.Descricao,
                DataHoraInicio = a.DataHoraInicio,
                DataEstimadaTermino = a.DataEstimadaTermino,
                Fonte = a.Fonte,
                Status = a.Status
            }).ToList();

            foreach (var alerta in alertasDTO)
            {
                AddAlertaLinks(alerta);
            }

            return Ok(alertasDTO);
        }

        /// <summary>
        /// Busca alertas por tipo.
        /// </summary>
        /// <param name="tipo">Tipo do alerta para filtrar.</param>
        /// <returns>Retorna os alertas do tipo especificado.</returns>
        [HttpGet("por-tipo/{tipo}", Name = "GetAlertasPorTipo")]
        public async Task<ActionResult<IEnumerable<AlertaDTO>>> GetAlertasPorTipo(string tipo)
        {
            var alertas = await _alertaRepository.GetAlertasPorTipo(tipo);
            var alertasDTO = alertas.Select(a => new AlertaDTO
            {
                AlertaId = a.AlertaId,
                Tipo = a.Tipo,
                Severidade = a.Severidade,
                AreaGeografica = a.AreaGeografica,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                RaioAfetado = a.RaioAfetado,
                Descricao = a.Descricao,
                DataHoraInicio = a.DataHoraInicio,
                DataEstimadaTermino = a.DataEstimadaTermino,
                Fonte = a.Fonte,
                Status = a.Status
            }).ToList();

            foreach (var alerta in alertasDTO)
            {
                AddAlertaLinks(alerta);
            }

            return Ok(alertasDTO);
        }
        
        /// <summary>
        /// Realiza uma previsão de alerta usando o modelo ML.NET.
        /// </summary>
        /// <param name="input">Dados de entrada para a previsão do ML.</param>
        /// <returns>O resultado da previsão do ML.</returns>
        [HttpPost("predict-ml")]
        public IActionResult PredictMl([FromBody] DisasterData input)
        {
            try
            {
                var prediction = _mlService.Predict(input);
                return Ok(prediction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao realizar previsão com ML.NET: " + ex.Message });
            }
        }

        private void AddAlertaLinks(AlertaDTO alertaDTO)
        {
            alertaDTO.Links.Add(new Link(Url.Link("GetAlerta", new { id = alertaDTO.AlertaId }), "self", "GET"));
            alertaDTO.Links.Add(new Link(Url.Link("UpdateAlerta", new { id = alertaDTO.AlertaId }), "update", "PUT"));
            alertaDTO.Links.Add(new Link(Url.Link("DeleteAlerta", new { id = alertaDTO.AlertaId }), "delete", "DELETE"));
            alertaDTO.Links.Add(new Link(Url.Link("UpdateAlertaStatus", new { id = alertaDTO.AlertaId }), "update-status", "PATCH"));
        }
    }
}