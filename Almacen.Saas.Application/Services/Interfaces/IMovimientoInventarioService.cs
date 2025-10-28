using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.MovimientoInventario;
using Almacen.Saas.Application.DTOs.Producto;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IMovimientoInventarioService
{
    Task<Result<MovimientoInventarioDto>> CrearAsync(CrearMovimientoInventarioDto dto);

    Task<Result<PagedResultDto<MovimientoInventarioDto>>> ObtenerTodosAsync(int page, int pageSize);
    Task<Result<MovimientoInventarioDto>> ObtenerMovimientoAsync(int id);
    Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorProductoAsync(int productoId);
    Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorFechaAsync(DateTime fecha);
    Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorTipoAsync(int tipo);
    Task<Result<int>> CalcularInventarioActualAsync(int productoId);
    Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosEntreFechasAsync(DateTime desde, DateTime hasta);
    Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosConDetallesAsync(int productoId, DateTime desde, DateTime hasta, int tipo);
    Task<Result<List<ProductoDto>>> ObtenerProductosConStockBajoAsync(int limite = 10);


}
