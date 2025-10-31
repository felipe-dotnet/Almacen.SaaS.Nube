using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Dashboard;
using Almacen.Saas.Application.DTOs.Pedido;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IDashboardService
{
    /// <summary>
    /// Obtiene un dashboard completo con todas las estadísticas y alertas
    /// </summary>
    /// <param name="usuarioId">ID del usuario que solicita el dashboard</param>
    /// <returns>Dashboard con estadísticas integradas</returns>
    Task<Result<DashboardDto>> ObtenerDashboard(int usuarioId);

    /// <summary>
    /// Obtiene estadísticas de ventas para un período específico
    /// </summary>
    /// <param name="desde">Fecha inicial del período</param>
    /// <param name="hasta">Fecha final del período</param>
    /// <returns>Estadísticas de ventas (total, promedio, facturas pagadas, etc.)</returns>
    Task<Result<EstadisticasVentasDto>> ObtenerEstadisticasVentas(DateTime desde, DateTime hasta);

    /// <summary>
    /// Obtiene estadísticas del inventario actual
    /// </summary>
    /// <returns>Estadísticas de inventario (stock, productos sin stock, valor total, etc.)</returns>
    Task<Result<EstadisticasInventarioDto>> ObtenerEstadisticasInventario();

    /// <summary>
    /// Obtiene estadísticas de pedidos para un período específico
    /// </summary>
    /// <param name="desde">Fecha inicial del período</param>
    /// <param name="hasta">Fecha final del período</param>
    /// <returns>Estadísticas de pedidos (entregados, pendientes, tasa de entrega, etc.)</returns>
    Task<Result<EstadisticasPedidosDto>> ObtenerEstadisticasPedidos(DateTime desde, DateTime hasta);

    /// <summary>
    /// Obtiene los productos más vendidos
    /// </summary>
    /// <param name="top">Cantidad de productos a retornar (default: 10)</param>
    /// <returns>Lista de productos más vendidos con cantidad e ingreso</returns>
    Task<Result<List<ProductoMasVendidoDto>>> ObtenerProductosMasVendidos(int top = 10);

    /// <summary>
    /// Obtiene todas las alertas del sistema
    /// Incluye: stock bajo, sin stock, pedidos pendientes, facturas pendientes, etc.
    /// </summary>
    /// <returns>Lista de alertas con tipo, mensaje y severidad</returns>
    Task<Result<List<AlertaDto>>> ObtenerAlertas();

    /// <summary>
    /// Obtiene un resumen financiero completo para un período
    /// </summary>
    /// <param name="desde">Fecha inicial del período</param>
    /// <param name="hasta">Fecha final del período</param>
    /// <returns>Diccionario con datos financieros (ingresos, pendientes, IVA, etc.)</returns>
    Task<Result<Dictionary<string, object>>> ObtenerResumenFinanciero(DateTime desde, DateTime hasta);
}
