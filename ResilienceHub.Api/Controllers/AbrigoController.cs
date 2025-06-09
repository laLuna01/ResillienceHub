using Microsoft.AspNetCore.Mvc;
using ResilienceHub.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.RateLimiting;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Api.Controllers
{
    /// <summary>
    /// Controlador para gerenciar operações relacionadas a abrigos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AbrigosController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;

        /// <summary>
        /// Construtor do AbrigoController.
        /// </summary>
        /// <param name="abrigoRepository">Repositorio de abrigo.</param>
        public AbrigosController(IAbrigoRepository abrigoRepository)
        {
            _abrigoRepository = abrigoRepository;
        }
        
        /// <summary>
        /// Busca todos os abrigos no sistema.
        /// </summary>
        /// <returns>Retorna os abrigos cadastrados.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AbrigoDTO>>> GetAbrigos()
        {
            var abrigos = await _abrigoRepository.GetAbrigos();
            var abrigosDTO = abrigos.Select(a => new AbrigoDTO
            {
                AbrigoId = a.AbrigoId,
                Nome = a.Nome,
                Localizacao = a.Localizacao,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                Capacidade = a.Capacidade,
                OcupacaoAtual = a.OcupacaoAtual,
                DataCadastro = a.DataCadastro,
                Ativo = a.Ativo
            }).ToList();

            foreach (var abrigo in abrigosDTO)
            {
                abrigo.Links.Add(new Link(Url.Link("GetAbrigo", new { id = abrigo.AbrigoId }), "self", "GET"));
                abrigo.Links.Add(new Link(Url.Link("UpdateAbrigo", new { id = abrigo.AbrigoId }), "update", "PUT"));
                abrigo.Links.Add(new Link(Url.Link("DeleteAbrigo", new { id = abrigo.AbrigoId }), "delete", "DELETE"));
            }

            return Ok(abrigosDTO);
        }

        /// <summary>
        /// Insere um novo abrigo no sistema.
        /// </summary>
        /// <param name="abrigo">Dados do abrigo a ser inserido.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpPost]
        public async Task<ActionResult<AbrigoDTO>> PostAbrigo([FromBody] Abrigo abrigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAbrigo = await _abrigoRepository.CreateAbrigo(abrigo);

            var abrigoDTO = new AbrigoDTO
            {
                AbrigoId = createdAbrigo.AbrigoId,
                Nome = createdAbrigo.Nome,
                Localizacao = createdAbrigo.Localizacao,
                Latitude = createdAbrigo.Latitude,
                Longitude = createdAbrigo.Longitude,
                Capacidade = createdAbrigo.Capacidade,
                OcupacaoAtual = createdAbrigo.OcupacaoAtual,
                DataCadastro = createdAbrigo.DataCadastro,
                Ativo = createdAbrigo.Ativo
            };

            abrigoDTO.Links.Add(new Link(Url.Link("GetAbrigo", new { id = abrigoDTO.AbrigoId }), "self", "GET"));
            abrigoDTO.Links.Add(new Link(Url.Link("UpdateAbrigo", new { id = abrigoDTO.AbrigoId }), "update", "PUT"));
            abrigoDTO.Links.Add(new Link(Url.Link("DeleteAbrigo", new { id = abrigoDTO.AbrigoId }), "delete", "DELETE"));

            return CreatedAtRoute("GetAbrigo", new { id = abrigoDTO.AbrigoId }, abrigoDTO);
        }

        /// <summary>
        /// Atualiza um abrigo existente no sistema.
        /// </summary>
        /// <param name="id">ID do abrigo a ser atualizado.</param>
        /// <param name="abrigo">Novos dados do abrigo.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpPut("{id}", Name = "UpdateAbrigo")]
        public async Task<IActionResult> PutAbrigo(int id, [FromBody] Abrigo abrigo)
        {
            if (id != abrigo.AbrigoId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _abrigoRepository.UpdateAbrigo(abrigo);

            return NoContent();
        }

        /// <summary>
        /// Exclui um abrigo do sistema.
        /// </summary>
        /// <param name="id">ID do abrigo a ser excluído.</param>
        /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
        [HttpDelete("{id}", Name = "DeleteAbrigo")]
        public async Task<IActionResult> DeleteAbrigo(int id)
        {
            await _abrigoRepository.DeleteAbrigo(id);
            return NoContent();
        }

        /// <summary>
        /// Obtém um abrigo pelo seu ID.
        /// </summary>
        /// <param name="id">ID do abrigo.</param>
        /// <returns>O abrigo correspondente ou NotFound se não for encontrado.</returns>
        [HttpGet("{id}", Name = "GetAbrigo")]
        public async Task<ActionResult<AbrigoDTO>> GetAbrigo(int id)
        {
            var abrigo = await _abrigoRepository.GetAbrigo(id);

            if (abrigo == null)
            {
                return NotFound();
            }

            var abrigoDTO = new AbrigoDTO
            {
                AbrigoId = abrigo.AbrigoId,
                Nome = abrigo.Nome,
                Localizacao = abrigo.Localizacao,
                Latitude = abrigo.Latitude,
                Longitude = abrigo.Longitude,
                Capacidade = abrigo.Capacidade,
                OcupacaoAtual = abrigo.OcupacaoAtual,
                DataCadastro = abrigo.DataCadastro,
                Ativo = abrigo.Ativo
            };

            abrigoDTO.Links.Add(new Link(Url.Link("GetAbrigo", new { id = abrigoDTO.AbrigoId }), "self", "GET"));
            abrigoDTO.Links.Add(new Link(Url.Link("UpdateAbrigo", new { id = abrigoDTO.AbrigoId }), "update", "PUT"));
            abrigoDTO.Links.Add(new Link(Url.Link("DeleteAbrigo", new { id = abrigoDTO.AbrigoId }), "delete", "DELETE"));

            return Ok(abrigoDTO);
        }
    }
}


