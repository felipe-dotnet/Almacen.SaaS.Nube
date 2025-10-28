using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.MovimientoInventario;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IMovimientoService
{
    Task<Result<PagedResultDto<MovimientoInventarioDto>>> ObtenerTodosAsync(int page, int pageSize);
    Task<Result<int>> CrearAsync(CrearMovimientoInventarioDto dto);
}
