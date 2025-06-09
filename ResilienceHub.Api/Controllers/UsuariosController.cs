using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResilienceHub.Core.Models;
using ResilienceHub.Infrastructure.Interfaces;

namespace ResilienceHub.Api.Controllers;

/// <summary>
/// Controlador para gerenciar operações relacionadas a usuários.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    /// <summary>
    /// Construtor do UsuariosController.
    /// </summary>
    /// <param name="usuarioRepository">Repositório de usuário.</param>
    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
    
    /// <summary>
    /// Busca todos os usuários no sistema.
    /// </summary>
    /// <returns>Retorna os usuários cadastrados.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
    {
        var usuarios = await _usuarioRepository.GetUsuarios();
        var usuariosDTO = usuarios.Select(u => new UsuarioDTO
        {
            UsuarioId = u.UsuarioId,
            Nome = u.Nome,
            Cpf = u.Cpf,
            Tipo = u.Tipo,
            Email = u.Email,
            Telefone = u.Telefone,
            Latitude = u.Latitude,
            Longitude = u.Longitude,
            DataUltimaLocalizacao = u.DataUltimaLocalizacao,
            DataCadastro = u.DataCadastro
        }).ToList();

        foreach (var usuario in usuariosDTO)
        {
            AddUsuarioLinks(usuario);
        }

        return Ok(usuariosDTO);
    }

    /// <summary>
    /// Obtém um usuário pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    /// <returns>O usuário correspondente ou NotFound se não for encontrado.</returns>
    [HttpGet("{id}", Name = "GetUsuario")]
    public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
    {
        var usuario = await _usuarioRepository.GetUsuario(id);

        if (usuario == null)
        {
            return NotFound();
        }

        var usuarioDTO = new UsuarioDTO
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Cpf = usuario.Cpf,
            Tipo = usuario.Tipo,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Latitude = usuario.Latitude,
            Longitude = usuario.Longitude,
            DataUltimaLocalizacao = usuario.DataUltimaLocalizacao,
            DataCadastro = usuario.DataCadastro
        };

        AddUsuarioLinks(usuarioDTO);

        return Ok(usuarioDTO);
    }

    /// <summary>
    /// Obtém um usuário pelo CPF.
    /// </summary>
    /// <param name="cpf">CPF do usuário.</param>
    /// <returns>O usuário correspondente ou NotFound se não for encontrado.</returns>
    [HttpGet("por-cpf/{cpf}", Name = "GetUsuarioByCpf")]
    public async Task<ActionResult<UsuarioDTO>> GetUsuarioByCpf(string cpf)
    {
        var usuario = await _usuarioRepository.GetUsuarioByCpf(cpf);

        if (usuario == null)
        {
            return NotFound();
        }

        var usuarioDTO = new UsuarioDTO
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Cpf = usuario.Cpf,
            Tipo = usuario.Tipo,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Latitude = usuario.Latitude,
            Longitude = usuario.Longitude,
            DataUltimaLocalizacao = usuario.DataUltimaLocalizacao,
            DataCadastro = usuario.DataCadastro
        };

        AddUsuarioLinks(usuarioDTO);

        return Ok(usuarioDTO);
    }

    /// <summary>
    /// Insere um novo usuário no sistema.
    /// </summary>
    /// <param name="usuario">Dados do usuário a ser inserido.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> PostUsuario([FromBody] Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verifica se CPF já existe
        if (await _usuarioRepository.GetUsuarioByCpf(usuario.Cpf) != null)
        {
            return Conflict("CPF já cadastrado");
        }

        // Verifica se Email já existe
        if (await _usuarioRepository.GetUsuarioByEmail(usuario.Email) != null)
        {
            return Conflict("Email já cadastrado");
        }

        var createdUsuario = await _usuarioRepository.CreateUsuario(usuario);

        var usuarioDTO = new UsuarioDTO
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Cpf = usuario.Cpf,
            Tipo = usuario.Tipo,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Latitude = usuario.Latitude,
            Longitude = usuario.Longitude,
            DataUltimaLocalizacao = usuario.DataUltimaLocalizacao,
            DataCadastro = usuario.DataCadastro
        };

        AddUsuarioLinks(usuarioDTO);

        return CreatedAtRoute("GetUsuario", new { id = usuarioDTO.UsuarioId }, usuarioDTO);
    }

    /// <summary>
    /// Atualiza um usuário existente no sistema.
    /// </summary>
    /// <param name="id">ID do usuário a ser atualizado.</param>
    /// <param name="usuario">Novos dados do usuário.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpPut("{id}", Name = "UpdateUsuario")]
    public async Task<IActionResult> PutUsuario(int id, [FromBody] Usuario usuario)
    {
        if (id != usuario.UsuarioId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _usuarioRepository.UpdateUsuario(usuario);

        return NoContent();
    }

    /// <summary>
    /// Exclui um usuário do sistema.
    /// </summary>
    /// <param name="id">ID do usuário a ser excluído.</param>
    /// <returns>Um IActionResult indicando o sucesso ou falha da operação.</returns>
    [HttpDelete("{id}", Name = "DeleteUsuario")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        await _usuarioRepository.DeleteUsuario(id);
        return NoContent();
    }

    private void AddUsuarioLinks(UsuarioDTO usuarioDTO)
    {
        usuarioDTO.Links.Add(new Link(Url.Link("GetUsuario", new { id = usuarioDTO.UsuarioId }), "self", "GET"));
        usuarioDTO.Links.Add(new Link(Url.Link("UpdateUsuario", new { id = usuarioDTO.UsuarioId }), "update", "PUT"));
        usuarioDTO.Links.Add(new Link(Url.Link("DeleteUsuario", new { id = usuarioDTO.UsuarioId }), "delete", "DELETE"));
    }
}