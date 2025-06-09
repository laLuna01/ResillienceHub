using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Api.Controllers;

/// <summary>
/// Controlador para gerenciar operações de check-in e check-out em abrigos
/// </summary>
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
public class CheckInController : ControllerBase
{
    private readonly IAbrigoUsuarioRepository _abrigoUsuarioRepository;
    private readonly IAbrigoRepository _abrigoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    /// <summary>
    /// Construtor do CheckInController
    /// </summary>
    /// <param name="abrigoUsuarioRepository">Repositório de relacionamento abrigo-usuário</param>
    /// <param name="abrigoRepository">Repositório de abrigos</param>
    /// <param name="usuarioRepository">Repositório de usuários</param>
    public CheckInController(
        IAbrigoUsuarioRepository abrigoUsuarioRepository,
        IAbrigoRepository abrigoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _abrigoUsuarioRepository = abrigoUsuarioRepository;
        _abrigoRepository = abrigoRepository;
        _usuarioRepository = usuarioRepository;    
    }

    /// <summary>
    /// Lista todos os registros de check-in/check-out
    /// </summary>
    /// <returns>Lista de todos os registros de check-in/check-out</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbrigoUsuarioDTO>>> GetRegistros()
    {
        var registros = await _abrigoUsuarioRepository.GetRegistros();
        var registrosDTO = new List<AbrigoUsuarioDTO>();
        foreach (var registro in registros)
        {
            registrosDTO.Add(await MapToDTO(registro));
        }
        foreach (var registro in registrosDTO)
        {
            AddLinks(registro);
        }

        return Ok(registrosDTO);
    }

    /// <summary>
    /// Lista registros de check-in ativos por abrigo
    /// </summary>
    /// <param name="abrigoId">ID do abrigo</param>
    /// <returns>Lista de registros ativos no abrigo especificado</returns>
    [HttpGet("abrigo/{abrigoId}")]
    public async Task<ActionResult<IEnumerable<AbrigoUsuarioDTO>>> GetRegistrosPorAbrigo(int abrigoId)
    {
        var abrigo = await _abrigoRepository.GetAbrigo(abrigoId);
        
        Console.WriteLine(abrigo);
        
        if (abrigo == null)
        {
            return NotFound("Abrigo não encontrado");
        }

        var registros = await _abrigoUsuarioRepository.GetRegistrosAtivosPorAbrigo(abrigoId);
        var registrosDTO = new List<AbrigoUsuarioDTO>();
        foreach (var registro in registros)
        {
            registrosDTO.Add(await MapToDTO(registro));
        }
        foreach (var registro in registrosDTO)
        {
            AddLinks(registro);
        }

        return Ok(registrosDTO);
    }

    /// <summary>
    /// Lista registros de check-in por usuário
    /// </summary>
    /// <param name="usuarioId">ID do usuário</param>
    /// <returns>Lista de registros do usuário especificado</returns>
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<AbrigoUsuarioDTO>>> GetRegistrosPorUsuario(int usuarioId)
    {
        var usuario = await _usuarioRepository.GetUsuario(usuarioId);
        if (usuario == null)
        {
            return NotFound("Usuário não encontrado");
        }

        var registros = await _abrigoUsuarioRepository.GetRegistrosPorUsuario(usuarioId);
        var registrosDTO = new List<AbrigoUsuarioDTO>();
        foreach (var registro in registros)
        {
            registrosDTO.Add(await MapToDTO(registro));
        }

        foreach (var registro in registrosDTO)
        {
            AddLinks(registro);
        }

        return Ok(registrosDTO);
    }

    /// <summary>
    /// Realiza o check-in de um usuário em um abrigo
    /// </summary>
    /// <param name="checkInRequest">Dados do check-in contendo AbrigoId e UsuarioId</param>
    /// <returns>Registro de check-in criado</returns>
    [HttpPost]
    public async Task<ActionResult<AbrigoUsuarioDTO>> PostCheckIn([FromBody] CheckInRequest checkInRequest)
    {
        // Verifica se o abrigo existe
        var abrigo = await _abrigoRepository.GetAbrigo(checkInRequest.AbrigoId);
        if (abrigo == null)
        {
            return NotFound("Abrigo não encontrado");
        }

        // Verifica se o usuário existe
        var usuario = await _usuarioRepository.GetUsuario(checkInRequest.UsuarioId);
        if (usuario == null)
        {
            return NotFound("Usuário não encontrado");
        }

        // Verifica se há vagas disponíveis
        if (abrigo.OcupacaoAtual >= abrigo.Capacidade)
        {
            return BadRequest("Abrigo está com capacidade máxima");
        }

        // Verifica se o usuário já está em algum abrigo
        var registrosAtivos = await _abrigoUsuarioRepository.GetRegistrosPorUsuario(checkInRequest.UsuarioId);
        if (registrosAtivos.Any(r => r.Status == "ativo"))
        {
            return BadRequest("Usuário já está registrado em outro abrigo");
        }

        var registro = new AbrigoUsuario
        {
            AbrigoId = checkInRequest.AbrigoId,
            UsuarioId = checkInRequest.UsuarioId,
            Status = "ativo"
        };

        var createdRegistro = await _abrigoUsuarioRepository.CreateCheckIn(registro);
        var registroDTO = await MapToDTO(createdRegistro);
        AddLinks(registroDTO);

        return CreatedAtAction(nameof(GetRegistro), new { id = registroDTO.RegistroId }, registroDTO);
    }

    /// <summary>
    /// Realiza o check-out de um usuário de um abrigo
    /// </summary>
    /// <param name="registroId">ID do registro de check-in</param>
    /// <returns>Registro de check-in atualizado com data de check-out</returns>
    [HttpPut("checkout/{registroId}")]
    public async Task<IActionResult> PutCheckOut(int registroId)
    {
        var registro = await _abrigoUsuarioRepository.GetRegistro(registroId);
        if (registro == null)
        {
            return NotFound();
        }

        if (registro.Status == "finalizado")
        {
            return BadRequest("Check-out já realizado anteriormente");
        }

        var updatedRegistro = await _abrigoUsuarioRepository.UpdateCheckOut(registroId);
        var registroDTO = await MapToDTO(updatedRegistro);
        AddLinks(registroDTO);

        return Ok(registroDTO);
    }

    /// <summary>
    /// Obtém um registro específico de check-in/check-out
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <returns>Registro completo com informações de abrigo e usuário</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AbrigoUsuarioDTO>> GetRegistro(int id)
    {
        var registro = await _abrigoUsuarioRepository.GetRegistro(id);

        if (registro == null)
        {
            return NotFound();
        }

        var registroDTO = await MapToDTO(registro);
        AddLinks(registroDTO);

        return Ok(registroDTO);
    }

    /// <summary>
    /// Mapeia um objeto AbrigoUsuario para AbrigoUsuarioDTO
    /// </summary>
    /// <param name="registro">Objeto AbrigoUsuario a ser mapeado</param>
    /// <returns>Objeto AbrigoUsuarioDTO mapeado</returns>
    private async Task<AbrigoUsuarioDTO> MapToDTO(AbrigoUsuario registro)
{
    // Busca os nomes do abrigo e usuário separadamente
    var abrigo = await _abrigoRepository.GetAbrigo(registro.AbrigoId);
    var usuario = await _usuarioRepository.GetUsuario(registro.UsuarioId);

    return new AbrigoUsuarioDTO
    {
        RegistroId = registro.RegistroId,
        AbrigoId = registro.AbrigoId,
        UsuarioId = registro.UsuarioId,
        DataCheckin = registro.DataCheckin,
        DataCheckout = registro.DataCheckout,
        Status = registro.Status,
        NomeAbrigo = abrigo?.Nome,
        NomeUsuario = usuario?.Nome
    };
}

    /// <summary>
    /// Adiciona links HATEOAS a um DTO de AbrigoUsuario
    /// </summary>
    /// <param name="registroDTO">DTO que receberá os links</param>
    private void AddLinks(AbrigoUsuarioDTO registroDTO)
    {
        registroDTO.Links.Add(new Link(Url.Link("GetRegistro", new { id = registroDTO.RegistroId }), "self", "GET"));
        
        if (registroDTO.Status == "ativo")
        {
            registroDTO.Links.Add(new Link(Url.Link("CheckOut", new { registroId = registroDTO.RegistroId }), "checkout", "PUT"));
        }
    }
}