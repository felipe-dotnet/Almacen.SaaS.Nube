using Almacen.Saas.Application.DTOs.Usuario;
using Almacen.Saas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Almacen.Saas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios (paginado)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation($"Obteniendo usuarios - Página {pageNumber}, Tamaño {pageSize}");
        var result = await _usuarioService.ObtenerTodosAsync(pageNumber, pageSize);

        if (!result.Success)
            return BadRequest(result.Message, result.Errors);

        return OkPaginado(result.Data);
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        _logger.LogInformation($"Obteniendo usuario {id}");
        var result = await _usuarioService.ObtenerPorIdAsync(id);

        if (!result.Success)
            return NotFound(new { success = false, message = result.Message });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un usuario por email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<IActionResult> ObtenerPorEmail(string email)
    {
        _logger.LogInformation($"Obteniendo usuario por email: {email}");
        var result = await _usuarioService.ObtenerPorEmailAsync(email);

        if (!result.Success)
            return NotFound(new { success = false, message = result.Message });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioDto dto)
    {
        _logger.LogInformation($"Creando nuevo usuario: {dto.Email}");
        var result = await _usuarioService.CrearAsync(dto);

        if (!result.Success)
            return BadRequest(result.Message, result.Errors);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data?.Id }, result.Data);
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
    {
        _logger.LogInformation($"Actualizando usuario {id}");
        var result = await _usuarioService.ActualizarAsync(dto);

        if (!result.Success)
            return BadRequest(result.Message, result.Errors);

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un usuario
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        _logger.LogInformation($"Eliminando usuario {id}");
        var result = await _usuarioService.EliminarAsync(id);

        if (!result.Success)
            return BadRequest(result.Message, result.Errors);

        return Ok(new { success = true, message = result.Message });
    }

    /// <summary>
    /// Cambia la contraseña de un usuario
    /// </summary>
    [HttpPost("{id}/cambiar-password")]
    public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordDto dto)
    {
        _logger.LogInformation($"Cambiando contraseña del usuario {id}");
        var result = await _usuarioService.CambiarPasswordAsync(dto);

        if (!result.Success)
            return BadRequest(result.Message, result.Errors);

        return Ok(new { success = true, message = result.Message });
    }
}
