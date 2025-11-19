using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almacen.Saas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Retorna un response exitoso
    /// </summary>
    protected IActionResult Ok<T>(T data, string message = "Operación exitosa")
    {
        return base.Ok(new { success = true, data, message }); // ← Agregado "base."
    }

    /// <summary>
    /// Retorna un response con error
    /// </summary>
    protected IActionResult BadRequest(string message, List<string>? errors = null)
    {
        return base.BadRequest(new { success = false, message, errors = errors ?? [] }); // ← Agregado "base."
    }

    /// <summary>
    /// Retorna un response paginado
    /// </summary>
    protected IActionResult OkPaginado<T>(T data, string message = "Operación exitosa")
    {
        return base.Ok(new { success = true, data, message }); // ← Agregado "base."
    }
}