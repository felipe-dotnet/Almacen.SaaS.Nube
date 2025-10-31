using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Dashboard;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Almacen.Saas.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;

public class DashboardService : IDashboardService
{
    #region Constructor
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    #endregion

    public async Task<Result<DashboardDto>> ObtenerDashboard(int usuarioId)
    {
        try
        {
            // 1. Validar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId) ?? throw new Exception("Usuario no encontrado");

            // 2. Obtener datos del dashboard
            var hoy = DateTime.UtcNow.Date;
            var hace30Dias = hoy.AddDays(-30);
            var hace90Dias = hoy.AddDays(-90);

            // Obtener estadísticas
            var estadisticasVentas = await ObtenerEstadisticasVentas(hace30Dias, hoy);
            var estadisticasInventario = await ObtenerEstadisticasInventario();
            var estadisticasPedidos = await ObtenerEstadisticasPedidos(hace30Dias, hoy);
            var productosMasVendidos = await ObtenerProductosMasVendidos(5);
            var alertas = await ObtenerAlertas();

            // 3. Construir dashboard
            var dashboard = new DashboardDto
            {
                //FechaActualizacion = DateTime.UtcNow,
                Ventas = estadisticasVentas.Data ?? new EstadisticasVentasDto(),
                Inventario = estadisticasInventario.Data??new EstadisticasInventarioDto(),
                Pedidos = estadisticasPedidos.Data ?? new EstadisticasPedidosDto(),
                ProductosMasVendidos = productosMasVendidos.Data ?? [],
                Alertas = alertas.Data ?? []
            };

            _logger.LogInformation($"Dashboard obtenido para usuario {usuarioId}");
            return Result<DashboardDto>.SuccessResult(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener dashboard: {ex.Message}");
            return Result<DashboardDto>.FailureResult("Error al obtener dashboard", new List<string> { ex.Message });
        }
    }

    public async Task<Result<EstadisticasVentasDto>> ObtenerEstadisticasVentas(DateTime desde, DateTime hasta)
    {
        try
        {
            // Validar fechas
            if (desde > hasta)
            {
                throw new Exception("La fecha 'desde' no puede ser mayor que 'hasta'");
            }

            var inicioDelDia = desde.Date;
            var finDelDia = hasta.Date.AddDays(1).AddTicks(-1);

            // Obtener facturas en el rango
            var facturas = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.FechaCreacion >= inicioDelDia && f.FechaCreacion <= finDelDia);

            var totalVentas = facturas.Sum(f => f.Total);
            var totalFacturas = facturas.Count();
            var ventaPromedio = totalFacturas > 0 ? totalVentas / totalFacturas : 0;

            // Obtener facturas pagadas
            var facturasPagadas = facturas.Where(f => f.Activo).Count();

            // Agrupar por día para gráfico
            var ventasPorDia = facturas
                .GroupBy(f => f.FechaCreacion.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Fecha = g.Key,
                    Total = g.Sum(f => f.Total),
                    Cantidad = g.Count()
                })
                .ToList();

            var estadisticas = new EstadisticasVentasDto
            {
                VentasHoy = totalVentas,
                VentasMes = totalFacturas,
                VentasSemana = ventaPromedio,
                VentasAnio = facturasPagadas,
                PorcentajePago = totalFacturas > 0 ? (facturasPagadas * 100) / totalFacturas : 0,
                Periodo = $"{desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}"
            };

            _logger.LogInformation($"Estadísticas de ventas obtenidas: {totalVentas}");
            return Result<EstadisticasVentasDto>.SuccessResult(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de ventas: {ex.Message}");
            return Result<EstadisticasVentasDto>.FailureResult(
                "Error al obtener estadísticas de ventas", new List<string> { ex.Message });
        }
    }

    public async Task<Result<EstadisticasInventarioDto>> ObtenerEstadisticasInventario()
    {
        try
        {
            // Obtener todos los productos
            var productos = await _unitOfWork.Repository<Producto>().GetAllAsync();

            if (productos.Count() == 0)
            {
                return Result<EstadisticasInventarioDto>.SuccessResult(
                    new EstadisticasInventarioDto
                    {
                        TotalProductos = 0,
                        StockTotal = 0,
                        ProductosSinStock = 0,
                        ProductosStockBajo = 0,
                        ValorInventario = 0
                    });
            }

            // Calcular estadísticas
            var totalProductos = productos.Count();
            var stockTotal = productos.Sum(p => p.Stock);
            var productosSinStock = productos.Count(p => p.Stock == 0);
            var productosStockBajo = productos.Count(p => p.Stock > 0 && p.Stock <= 10);
            var valorInventario = productos.Sum(p => p.Stock * p.Precio);

            // Categoría con más stock
            var categoriaConMasStock = productos
                .GroupBy(p => p.UnidadMedida)
                .OrderByDescending(g => g.Sum(p => p.Stock))
                .FirstOrDefault()?
                .Key ?? 0;

            var estadisticas = new EstadisticasInventarioDto
            {
                TotalProductos = totalProductos,
                StockTotal = stockTotal,
                ProductosSinStock = productosSinStock,
                ProductosStockBajo = productosStockBajo,
                ValorInventario = valorInventario,
                StockPromedio = stockTotal / totalProductos,
            };

            _logger.LogInformation($"Estadísticas de inventario obtenidas: {totalProductos} productos");
            return Result<EstadisticasInventarioDto>.SuccessResult(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de inventario: {ex.Message}");
            return Result<EstadisticasInventarioDto>.FailureResult(
                "Error al obtener estadísticas de inventario", new List<string> { ex.Message });
        }
    }

    public async Task<Result<EstadisticasPedidosDto>> ObtenerEstadisticasPedidos(DateTime desde, DateTime hasta)
    {
        try
        {
            // Validar fechas
            if (desde > hasta)
            {
                throw new Exception("La fecha 'desde' no puede ser mayor que 'hasta'");
            }

            var inicioDelDia = desde.Date;
            var finDelDia = hasta.Date.AddDays(1).AddTicks(-1);

            // Obtener pedidos en el rango
            var pedidos = await _unitOfWork.Repository<Pedido>()
                .FindAsync(p => p.FechaPedido >= inicioDelDia && p.FechaPedido <= finDelDia);

            var totalPedidos = pedidos.Count();
            var pedidosEntregados = pedidos.Count(p => p.Estado == EstadoPedido.Entregado);
            var pedidosPendientes = pedidos.Count(p => p.Estado == EstadoPedido.Pendiente);
            var pedidosEnRuta = pedidos.Count(p => p.Estado == EstadoPedido.Enviado);
            var pedidosCancelados = pedidos.Count(p => p.Estado == EstadoPedido.Cancelado);

            var totalVentas = pedidos.Sum(p => p.Total);
            var ventaPromedio = totalPedidos > 0 ? totalVentas / totalPedidos : 0;

            // Tiempo promedio de entrega (si tiene fecha de actualización)
            var tiempoPromedio = pedidosEntregados > 0
                ? pedidos
                    .Where(p => p.Estado == EstadoPedido.Entregado && p.FechaModificacion.HasValue)
                    .Average(p => p.FechaModificacion.HasValue ? (p.FechaModificacion.Value - p.FechaPedido).TotalHours : 0) : 0;

            var estadisticas = new EstadisticasPedidosDto
            {
                TotalPedidos = totalPedidos,
                PedidosEntregados = pedidosEntregados,
                PedidosPendientes = pedidosPendientes,
                PedidosEnRuta = pedidosEnRuta,
                PedidosCancelados = pedidosCancelados,
                TasaEntrega = totalPedidos > 0 ? (pedidosEntregados * 100) / totalPedidos : 0,
                TotalVentas = totalVentas,
                VentaPromedio = ventaPromedio,
                TiempoPromedio = tiempoPromedio,
                Periodo = $"{desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}"
            };

            _logger.LogInformation($"Estadísticas de pedidos obtenidas: {totalPedidos} pedidos");
            return Result<EstadisticasPedidosDto>.SuccessResult(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de pedidos: {ex.Message}");
            return Result<EstadisticasPedidosDto>.FailureResult(
                "Error al obtener estadísticas de pedidos", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<ProductoMasVendidoDto>>> ObtenerProductosMasVendidos(int top = 10)
    {
        try
        {
            if (top <= 0)
            {
                throw new Exception("El valor de top debe ser mayor a 0");
            }

            // Obtener detalles de pedidos (ventas)
            var detalles = await _unitOfWork.Repository<DetallePedido>().GetAllAsync();

            // Agrupar por producto y ordenar
            var productosMasVendidos = detalles
                .GroupBy(d => d.ProductoId)
                .OrderByDescending(g => g.Sum(d => d.Cantidad))
                .Take(top)
                .ToList();

            var resultado = new List<ProductoMasVendidoDto>();

            foreach (var grupo in productosMasVendidos)
            {
                var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(grupo.Key);

                if (producto != null)
                {
                    resultado.Add(new ProductoMasVendidoDto
                    {
                        ProductoId = producto.Id,
                        NombreProducto = producto.Nombre,
                        CantidadVendida = grupo.Sum(d => (int)d.Cantidad),
                        TotalVentas = grupo.Sum(d => d.Subtotal),
                        Precio = producto.Precio
                    });
                }
            }

            _logger.LogInformation($"Productos más vendidos obtenidos: {resultado.Count}");
            return Result<List<ProductoMasVendidoDto>>.SuccessResult(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener productos más vendidos: {ex.Message}");
            return Result<List<ProductoMasVendidoDto>>.FailureResult(
                "Error al obtener productos más vendidos", new List<string> { ex.Message });
        }
    }
    public async Task<Result<List<AlertaDto>>> ObtenerAlertas()
    {
        try
        {
            var alertas = new List<AlertaDto>();

            // 1. Alertas de stock bajo
            var productosStockBajo = await _unitOfWork.Repository<Producto>()
                .FindAsync(p => p.Stock <= 10 && p.Stock > 0);

            foreach (var producto in productosStockBajo)
            {
                alertas.Add(new AlertaDto
                {
                    Tipo = "Stock Bajo",
                    Mensaje = $"El producto {producto.Nombre} tiene stock bajo: {producto.Stock} unidades",
                    Severidad = "Advertencia",
                    FechaAlerta = DateTime.UtcNow
                });
            }

            // 2. Alertas de sin stock
            var productosSinStock = await _unitOfWork.Repository<Producto>()
                .FindAsync(p => p.Stock == 0);

            foreach (var producto in productosSinStock)
            {
                alertas.Add(new AlertaDto
                {
                    Tipo = "Sin Stock",
                    Mensaje = $"El producto {producto.Nombre} no tiene stock disponible",
                    Severidad = "Crítica",
                    FechaAlerta = DateTime.UtcNow
                });
            }

            // 3. Alertas de pedidos pendientes
            var pedidosPendientes = await _unitOfWork.Repository<Pedido>()
                .FindAsync(p => p.Estado == EstadoPedido.Pendiente);

            if (pedidosPendientes.Count() > 5)
            {
                alertas.Add(new AlertaDto
                {
                    Tipo = "Pedidos Pendientes",
                    Mensaje = $"Hay {pedidosPendientes.Count()} pedidos pendientes de procesar",
                    Severidad = "Advertencia",
                    FechaAlerta = DateTime.UtcNow
                });
            }

            // 4. Alertas de facturas pendientes
            var facturasPendientes = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.Activo);

            if (facturasPendientes.Count() > 0)
            {
                var totalPendiente = facturasPendientes.Sum(f => f.Total);
                alertas.Add(new AlertaDto
                {
                    Tipo = "Facturas Pendientes",
                    Mensaje = $"Hay {facturasPendientes.Count()} facturas pendientes por un total de ${totalPendiente:F2}",
                    Severidad = "Advertencia",
                    FechaAlerta = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"Alertas obtenidas: {alertas.Count}");
            return Result<List<AlertaDto>>.SuccessResult(alertas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener alertas: {ex.Message}");
            return Result<List<AlertaDto>>.FailureResult(
                "Error al obtener alertas", new List<string> { ex.Message });
        }
    }

    // ==================== MÉTODOS AUXILIARES ====================

    /// <summary>
    /// Obtiene resumen financiero del período
    /// </summary>
    public async Task<Result<Dictionary<string, object>>> ObtenerResumenFinanciero(DateTime desde, DateTime hasta)
    {
        try
        {
            // Validar fechas
            if (desde > hasta)
            {
                throw new Exception("La fecha 'desde' no puede ser mayor que 'hasta'");
            }

            var inicioDelDia = desde.Date;
            var finDelDia = hasta.Date.AddDays(1).AddTicks(-1);

            // Obtener facturas pagadas
            var facturasPagadas = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.FechaCreacion >= inicioDelDia &&
                                  f.FechaCreacion <= finDelDia &&
                                  f.Activo==true);

            // Obtener facturas pendientes
            var facturasPendientes = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.FechaCreacion >= inicioDelDia &&
                                  f.FechaCreacion <= finDelDia &&
                                  f.Activo == true);

            var ingresoTotal = facturasPagadas.Sum(f => f.Total);
            var pendientePago = facturasPendientes.Sum(f => f.Total);
            var iva = facturasPagadas.Sum(f => f.IVA);

            var resumen = new Dictionary<string, object>
            {
                { "IngresoTotal", ingresoTotal },
                { "PendientePago", pendientePago },
                { "IVA", iva },
                { "Neto", ingresoTotal - iva },
                { "FacturasPagadas", facturasPagadas.Count() },
                { "FacturasPendientes", facturasPendientes.Count() },
                { "Periodo", $"{desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}" }
            };

            _logger.LogInformation($"Resumen financiero obtenido: ${ingresoTotal}");
            return Result<Dictionary<string, object>>.SuccessResult(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener resumen financiero: {ex.Message}");
            return Result<Dictionary<string, object>>.FailureResult(
                "Error al obtener resumen financiero", new List<string> { ex.Message });
        }
    }
}
