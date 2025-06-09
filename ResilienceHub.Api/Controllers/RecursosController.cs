using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Api.Controllers;

/// <summary>
/// Controlador para gerenciar operações relacionadas a recursos.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
public class RecursosController : ControllerBase
{
    private readonly IRecursoRepository _recursoRepository;

    /// <summary>
    /// Construtor do RecursosController.
    /// </summary>
    /// <param name="recursoRepository">Repositório de recurso.</param>
    public RecursosController(IRecursoRepository recursoRepository)
    {
        _recursoRepository = recursoRepository;
    }
    
    /// <summary>
    /// Busca todos os recursos no sistema.
    /// </summary>
    /// <returns>Retorna os recursos cadastrados.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecursoDTO>>> GetRecursos()
    {
        var recursos = await _recursoRepository.GetRecursos();
        var recursosDTO = recursos.Select(r => new RecursoDTO
        {
            RecursoId = r.RecursoId,
            Tipo = r.Tipo,
            Descricao = r.Descricao,
            Quantidade = r.Quantidade,
            Validade = r.Validade,
            UnidadeMedida = r.UnidadeMedida
        }).ToList();

        foreach (var recurso in recursosDTO)
        {
            AddRecursoLinks(recurso);
        }

        return Ok(recursosDTO);
    }

    /// <summary>
    /// Obtém um recurso pelo seu ID.
    /// </summary>
    /// <param name="id">ID do recurso.</param>
    /// <returns>O recurso correspondente ou NotFound se não for encontrado.</returns>
    [HttpGet("{id}", Name = "GetRecurso")]
    public async Task<ActionResult<RecursoDTO>> GetRecurso(int id)
    {
        var recurso = await _recursoRepository.GetRecurso(id);

        if (recurso == null)
        {
            return NotFound();
        }

        var recursoDTO = new RecursoDTO
        {
            RecursoId = recurso.RecursoId,
            Tipo = recurso.Tipo,
            Descricao = recurso.Descricao,
            Quantidade = recurso.Quantidade,
            Validade = recurso.Validade,
            UnidadeMedida = recurso.UnidadeMedida
        };

        AddRecursoLinks(recursoDTO);

        return Ok(recursoDTO);
    }

    /// <summary>
    /// Busca recursos por tipo.
    /// </summary>
    /// <param name="tipo">Tipo do recurso para filtrar.</param>
    /// <returns>Retorna os recursos do tipo especificado.</returns>
    [HttpGet("por-tipo/{tipo}", Name = "GetRecursosPorTipo")]
    public async Task<ActionResult<IEnumerable<RecursoDTO>>> GetRecursosPorTipo(string tipo)
    {
        var recursos = await _recursoRepository.GetRecursosPorTipo(tipo);
        var recursosDTO = recursos.Select(r => new RecursoDTO
        {
            RecursoId = r.RecursoId,
            Tipo = r.Tipo,
            Descricao = r.Descricao,
            Quantidade = r.Quantidade,
            Validade = r.Validade,
            UnidadeMedida = r.UnidadeMedida
        }).ToList();

        foreach (var recurso in recursosDTO)
        {
            AddRecursoLinks(recurso);
        }

        return Ok(recursosDTO);
    }

    /// <summary>
    /// Busca recursos vencidos.
    /// </summary>
    /// <returns>Retorna os recursos com data de validade expirada.</returns>
    [HttpGet("vencidos", Name = "GetRecursosVencidos")]
    public async Task<ActionResult<IEnumerable<RecursoDTO>>> GetRecursosVencidos()
    {
        var recursos = await _recursoRepository.GetRecursosVencidos();
        var recursosDTO = recursos.Select(r => new RecursoDTO
        {
            RecursoId = r.RecursoId,
            Tipo = r.Tipo,
            Descricao = r.Descricao,
            Quantidade = r.Quantidade,
            Validade = r.Validade,
            UnidadeMedida = r.UnidadeMedida
        }).ToList();

        foreach (var recurso in recursosDTO)
        {
            AddRecursoLinks(recurso);
        }

        return Ok(recursosDTO);
    }

    /// <summary>
    /// Busca recursos com baixo estoque.
    /// </summary>
    /// <param name="quantidadeMinima">Quantidade mínima para considerar baixo estoque (padrão: 5).</param>
    /// <returns>Retorna os recursos com quantidade igual ou abaixo do limite.</returns>
    [HttpGet("baixo-estoque", Name = "GetRecursosBaixoEstoque")]
    public async Task<ActionResult<IEnumerable<RecursoDTO>>> GetRecursosBaixoEstoque(int quantidadeMinima = 5)
    {
        var recursos = await _recursoRepository.GetRecursosBaixoEstoque(quantidadeMinima);
        var recursosDTO = recursos.Select(r => new RecursoDTO
        {
            RecursoId = r.RecursoId,
            Tipo = r.Tipo,
            Descricao = r.Descricao,
            Quantidade = r.Quantidade,
            Validade = r.Validade,
            UnidadeMedida = r.UnidadeMedida
        }).ToList();

        foreach (var recurso in recursosDTO)
        {
            AddRecursoLinks(recurso);
        }

        return Ok(recursosDTO);
    }

    /// <summary>
    /// Insere um novo recurso no sistema.
    /// </summary>
    /// <param name="recurso">Dados do recurso a ser inserido.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpPost]
    public async Task<ActionResult<RecursoDTO>> PostRecurso([FromBody] Recurso recurso)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdRecurso = await _recursoRepository.CreateRecurso(recurso);

        var recursoDTO = new RecursoDTO
        {
            RecursoId = createdRecurso.RecursoId,
            Tipo = createdRecurso.Tipo,
            Descricao = createdRecurso.Descricao,
            Quantidade = createdRecurso.Quantidade,
            Validade = createdRecurso.Validade,
            UnidadeMedida = createdRecurso.UnidadeMedida
        };

        AddRecursoLinks(recursoDTO);

        return CreatedAtRoute("GetRecurso", new { id = recursoDTO.RecursoId }, recursoDTO);
    }

    /// <summary>
    /// Atualiza um recurso existente no sistema.
    /// </summary>
    /// <param name="id">ID do recurso a ser atualizado.</param>
    /// <param name="recurso">Novos dados do recurso.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpPut("{id}", Name = "UpdateRecurso")]
    public async Task<IActionResult> PutRecurso(int id, [FromBody] Recurso recurso)
    {
        if (id != recurso.RecursoId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _recursoRepository.UpdateRecurso(recurso);

        return NoContent();
    }

    /// <summary>
    /// Ajusta a quantidade de um recurso.
    /// </summary>
    /// <param name="id">ID do recurso.</param>
    /// <param name="quantidade">Valor a ser adicionado/subtraído (use negativo para diminuir).</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpPatch("{id}/ajustar-quantidade", Name = "AjustarQuantidadeRecurso")]
    public async Task<IActionResult> PatchAjustarQuantidade(int id, [FromBody] int quantidade)
    {
        await _recursoRepository.AjustarQuantidadeRecurso(id, quantidade);
        return NoContent();
    }

    /// <summary>
    /// Exclui um recurso do sistema.
    /// </summary>
    /// <param name="id">ID do recurso a ser excluído.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpDelete("{id}", Name = "DeleteRecurso")]
    public async Task<IActionResult> DeleteRecurso(int id)
    {
        await _recursoRepository.DeleteRecurso(id);
        return NoContent();
    }

    private void AddRecursoLinks(RecursoDTO recursoDTO)
    {
        recursoDTO.Links.Add(new Link(Url.Link("GetRecurso", new { id = recursoDTO.RecursoId }), "self", "GET"));
        recursoDTO.Links.Add(new Link(Url.Link("UpdateRecurso", new { id = recursoDTO.RecursoId }), "update", "PUT"));
        recursoDTO.Links.Add(new Link(Url.Link("DeleteRecurso", new { id = recursoDTO.RecursoId }), "delete", "DELETE"));
        recursoDTO.Links.Add(new Link(Url.Link("AjustarQuantidadeRecurso", new { id = recursoDTO.RecursoId }), "adjust-quantity", "PATCH"));
    }
}