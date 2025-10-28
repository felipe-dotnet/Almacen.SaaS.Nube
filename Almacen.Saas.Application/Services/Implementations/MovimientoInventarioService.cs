using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.MovimientoInventario;
using Almacen.Saas.Application.DTOs.Producto;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;
public class MovimientoInventarioService : IMovimientoInventarioService
{
    #region Constructor
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MovimientoInventarioService> _logger;

    public MovimientoInventarioService(IUnitOfWork unitOfWork, ILogger<MovimientoInventarioService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #endregion
    public async Task<Result<MovimientoInventarioDto>> CrearAsync(CrearMovimientoInventarioDto dto)
    {
        try
        {
            // 1. Validar que el producto existe
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(dto.ProductoId) ?? throw new Exception("Producto no encontrado");

            // 2. Validar cantidad
            if (dto.Cantidad <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a 0");
            }

            // 3. Crear movimiento
            var movimiento = new MovimientoInventario
            {
                ProductoId = dto.ProductoId,
                TipoMovimiento = (TipoMovimiento)dto.TipoMovimiento,
                Cantidad = dto.Cantidad,
                Motivo = dto.Motivo,
                FechaCreacion = DateTime.UtcNow,
                Referencia = dto.Referencia
            };

            // 6. Actualizar stock del producto según tipo de movimiento
            switch ((TipoMovimiento)dto.TipoMovimiento)
            {
                case TipoMovimiento.Entrada:
                case TipoMovimiento.Devolucion:
                    producto.Stock += (int)dto.Cantidad;
                    break;
                case TipoMovimiento.Salida:
                case TipoMovimiento.Ajuste:
                    producto.Stock -= (int)dto.Cantidad;
                    break;
            }

            // Validar que el stock no sea negativo
            if (producto.Stock < 0)
            {
                throw new Exception(
                    "El movimiento resultaría en stock negativo");
            }

            await _unitOfWork.Repository<MovimientoInventario>().AddAsync(movimiento);
            await _unitOfWork.Repository<Producto>().UpdateAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            // 7. Crear alerta si el stock es bajo
            if (producto.Stock < 10) // TODO: Hacer configurable
            {
                var alerta = new Notificacion
                {
                    UsuarioId = 1, // TODO: Obtener usuario administrador
                    Tipo = TipoNotificacion.BajoInventario, //"Alerta de Stock Bajo",
                    Mensaje = $"El producto {producto.Nombre} tiene stock bajo ({producto.Stock} unidades)",
                    FechaCreacion = DateTime.UtcNow,
                    Leida = false
                };

                await _unitOfWork.Repository<Notificacion>().AddAsync(alerta);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation($"Movimiento {dto.TipoMovimiento.ToString()} registrado para producto {producto.Nombre}");

            var movimientoDto = movimiento.Adapt<MovimientoInventarioDto>();
            return Result<MovimientoInventarioDto>.SuccessResult(movimientoDto, "Movimiento registrado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear movimiento: {ex.Message}");
            return Result<MovimientoInventarioDto>.FailureResult("Error al crear movimiento", new List<string> { ex.Message });
        }
    }
    public async Task<Result<MovimientoInventarioDto>> ObtenerMovimientoAsync(int id)
    {
        try
        {
            var movimiento = await _unitOfWork.Repository<MovimientoInventario>().GetByIdAsync(id) ?? throw new Exception($"Movimiento con ID {id} no encontrado");
            var movimientoDto = movimiento.Adapt<MovimientoInventarioDto>();
            return Result<MovimientoInventarioDto>.SuccessResult(movimientoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimiento: {ex.Message}");
            return Result<MovimientoInventarioDto>.FailureResult("Error al obtener movimiento", [ex.Message]);
        }
    }
    public async Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorProductoAsync(int productoId)
    {
        try
        {
            // Verificar que el producto existe
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(productoId) ?? throw new Exception("Producto no encontrado");

            var movimientos = await _unitOfWork.Repository<MovimientoInventario>()
                .FindAsync(m => m.ProductoId == productoId);

            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            _logger.LogInformation($"Se obtuvieron {movimientos.Count()} movimientos para producto {productoId}");
            return Result<List<MovimientoInventarioDto>>.SuccessResult(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos por producto: {ex.Message}");
            return Result<List<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", [ex.Message]);
        }
    }
    public async Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorFechaAsync(DateTime fecha)
    {
        try
        {
            var inicioDelDia = fecha.Date;
            var finDelDia = inicioDelDia.AddDays(1).AddTicks(-1);

            var movimientos = await _unitOfWork.Repository<MovimientoInventario>()
                .FindAsync(m => m.FechaCreacion >= inicioDelDia && m.FechaCreacion <= finDelDia);

            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            _logger.LogInformation($"Se obtuvieron {movimientos.Count()} movimientos para {fecha:yyyy-MM-dd}");
            return Result<List<MovimientoInventarioDto>>.SuccessResult(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos por fecha: {ex.Message}");
            return Result<List<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosPorTipoAsync(int tipo)
    {
        try
        {
            var movimientos = await _unitOfWork.Repository<MovimientoInventario>()
                .FindAsync(m => m.TipoMovimiento == (TipoMovimiento)tipo);

            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            _logger.LogInformation($"Se obtuvieron {movimientos.Count()} movimientos de tipo {tipo}");
            return Result<List<MovimientoInventarioDto>>.SuccessResult(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos por tipo: {ex.Message}");
            return Result<List<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", new List<string> { ex.Message });
        }
    }
    public async Task<Result<int>> CalcularInventarioActualAsync(int productoId)
    {
        try
        {
            // Obtener producto
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(productoId) ?? throw new Exception("Producto no encontrado");

            // El stock actual del producto
            var inventarioActual = producto.Stock;

            _logger.LogInformation($"Inventario actual de producto {producto.Nombre}: {inventarioActual}");
            return Result<int>.SuccessResult(inventarioActual);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al calcular inventario: {ex.Message}");
            return Result<int>.FailureResult(
                "Error al calcular inventario", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosEntreFechasAsync(DateTime desde, DateTime hasta)
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

            var movimientos = await _unitOfWork.Repository<MovimientoInventario>()
                .FindAsync(m => m.FechaCreacion >= inicioDelDia && m.FechaCreacion <= finDelDia);

            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            _logger.LogInformation($"Se obtuvieron {movimientos.Count()} movimientos entre {desde:yyyy-MM-dd} y {hasta:yyyy-MM-dd}");
            return Result<List<MovimientoInventarioDto>>.SuccessResult(movimientosDto);
        }        
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos entre fechas: {ex.Message}");
            return Result<List<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", new List<string> { ex.Message });
        }
    }
    public async Task<Result<List<MovimientoInventarioDto>>> ObtenerMovimientosConDetallesAsync(int productoId, DateTime desde, DateTime hasta, int tipo)
    {
        try
        {
            var inicioDelDia = desde.Date;
            var finDelDia = hasta.Date.AddDays(1).AddTicks(-1);

            // Construir predicado dinámico
            var movimientos = string.IsNullOrEmpty(tipo.ToString())
                ? await _unitOfWork.Repository<MovimientoInventario>()
                    .FindAsync(m => m.ProductoId == productoId
                        && m.FechaCreacion >= inicioDelDia
                        && m.FechaCreacion <= finDelDia)
                : await _unitOfWork.Repository<MovimientoInventario>()
                    .FindAsync(m => m.ProductoId == productoId
                        && m.FechaCreacion >= inicioDelDia
                        && m.FechaCreacion <= finDelDia
                        && m.TipoMovimiento == (TipoMovimiento)tipo);

            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            _logger.LogInformation($"Se obtuvieron {movimientos.Count()} movimientos detallados");
            return Result<List<MovimientoInventarioDto>>.SuccessResult(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos detallados: {ex.Message}");
            return Result<List<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", new List<string> { ex.Message });
        }
    }
    public async Task<Result<PagedResultDto<MovimientoInventarioDto>>> ObtenerTodosAsync(int pageNumber, int pageSize)
    {
        try
        {
            // Validar parámetros
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<MovimientoInventario>().CountAsync();

            // Obtener movimientos paginados
            var movimientos = await _unitOfWork.Repository<MovimientoInventario>()
                .GetPaginatedAsync(skip, pageSize);

            // Mapear a DTO
            var movimientosDto = movimientos.Adapt<List<MovimientoInventarioDto>>();

            var result = new PagedResultDto<MovimientoInventarioDto>
            {
                Items = movimientosDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResultDto<MovimientoInventarioDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener movimientos: {ex.Message}");
            return Result<PagedResultDto<MovimientoInventarioDto>>.FailureResult(
                "Error al obtener movimientos", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<ProductoDto>>> ObtenerProductosConStockBajoAsync(int limite = 10)
    {
        try
        {
            if (limite <= 0)
            {
                throw new Exception("El límite debe ser mayor a 0");
            }

            var productos = await _unitOfWork.Repository<Producto>()
                .FindAsync(p => p.Stock <= limite);

            var productosDto = productos.Adapt<List<ProductoDto>>();

            _logger.LogInformation($"Se obtuvieron {productos.Count()} productos con stock bajo");
            return Result<List<ProductoDto>>.SuccessResult(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener productos con stock bajo: {ex.Message}");
            return Result<List<ProductoDto>>.FailureResult(
                "Error al obtener productos", new List<string> { ex.Message });
        }
    }
}
