using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Dashboard;
using Almacen.Saas.Application.DTOs.Pedido;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardDto>> ObtenerEstadisticasGeneralesAsync();
    Task<Result<EstadisticasInventarioDto>> ObtenerEstadoInventarioAsync();
    Task<Result<EstadisticasVentasDto>> ObtenerEstadisticasVentasAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<Result<EstadisticasPedidosDto>> ObtenerEstadisticasPedidosAsync();
    Task<Result<ProductoMasVendidoDto>> ObtenerProductoMasVendido(DateTime fechaInicio, DateTime fechaFin);
    
}
