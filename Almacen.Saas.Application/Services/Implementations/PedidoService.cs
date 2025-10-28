using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Pedido;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;
public class PedidoService : IPedidoService
{
    #region Constructor
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(IUnitOfWork unitOfWork, ILogger<PedidoService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    #endregion

    public async Task<Result<int>> CrearAsync(CrearPedidoDto dto)
    {
        try
        {
            // 1. Validar usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>()
                .GetByIdAsync(dto.UsuarioId) ?? throw new Exception("Usuario no encontrado");

            // 2. Validar que hay detalles
            if (dto.Detalles == null || dto.Detalles.Count == 0)
            {
                throw new Exception("El pedido debe tener al menos un producto");
            }

            // 3. Crear pedido
            var pedido = new Pedido
            {
                UsuarioId = dto.UsuarioId,
                FechaPedido = DateTime.UtcNow,
                Estado = Domain.Enums.EstadoPedido.Pendiente,
                Total = 0,
                Observaciones = dto.Observaciones ?? string.Empty
            };

            await _unitOfWork.Repository<Pedido>().AddAsync(pedido);
            await _unitOfWork.SaveChangesAsync();

            decimal totalPedido = 0;

            // 4. Procesar cada detalle del pedido
            foreach (var detalle in dto.Detalles)
            {
                // 4.1 Obtener producto
                var producto = await _unitOfWork.Repository<Producto>()
                    .GetByIdAsync(detalle.ProductoId) ?? throw new Exception(
                        $"Producto {detalle.ProductoId} no encontrado");

                // 4.2 Verificar stock disponible
                if (producto.Stock < detalle.Cantidad)
                {
                    throw new Exception(
                        $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}, Solicitado: {detalle.Cantidad}");
                }

                // 4.3 Crear detalle del pedido
                var detallePedido = new DetallePedido
                {
                    PedidoId = pedido.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * detalle.Cantidad
                };

                await _unitOfWork.Repository<DetallePedido>().AddAsync(detallePedido);

                // 4.4 Actualizar stock del producto
                producto.Stock -= (int)detalle.Cantidad;
                await _unitOfWork.Repository<Producto>().UpdateAsync(producto);

                // 4.5 Registrar movimiento de inventario (Salida)
                var movimiento = new MovimientoInventario
                {
                    ProductoId = detalle.ProductoId,
                    TipoMovimiento = Domain.Enums.TipoMovimiento.Salida,
                    Cantidad = detalle.Cantidad,
                    Motivo = $"Pedido #{pedido.Id}",
                    FechaCreacion = DateTime.UtcNow
                };

                await _unitOfWork.Repository<MovimientoInventario>().AddAsync(movimiento);

                totalPedido += detallePedido.Subtotal;
            }

            // 5. Actualizar total del pedido
            pedido.Total = totalPedido;
            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);

            // 6. Crear notificación al usuario
            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Tipo = TipoNotificacion.PedidoCreado,
                Mensaje = $"Tu pedido #{pedido.Id} ha sido creado exitosamente por un total de ${totalPedido:F2}",
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);

            // 7. Persistir todo y confirmar transacción
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation($"Pedido #{pedido.Id} creado exitosamente para usuario {dto.UsuarioId}");

            var pedidoDto = pedido.Adapt<PedidoDto>();
            return Result<int>.SuccessResult(pedidoDto.Id, "Pedido creado exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error al crear el pedido");
            return new Result<int>();
        }
    }
    public async Task<Result<PedidoDto>> ObtenerPorIdAsync(int id)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(id) ?? throw new Exception($"Pedido con ID {id} no encontrado");
            var pedidoDto = pedido.Adapt<PedidoDto>();
            return Result<PedidoDto>.SuccessResult(pedidoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener pedido: {ex.Message}");
            return Result<PedidoDto>.FailureResult("Error al obtener pedido", new List<string> { ex.Message });
        }
    }
    public async Task<Result<PagedResultDto<PedidoDto>>> ObtenerTodosAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (page - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<Pedido>().CountAsync();

            // Obtener pedidos paginados
            var pedidos = await _unitOfWork.Repository<Pedido>()
                .GetPaginatedAsync(skip, pageSize);

            // Mapear a DTO
            var pedidosDto = pedidos.Adapt<List<PedidoDto>>();

            var result = new PagedResultDto<PedidoDto>
            {
                Items = pedidosDto,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return Result<PagedResultDto<PedidoDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener pedidos: {ex.Message}");
            return Result<PagedResultDto<PedidoDto>>.FailureResult(
                "Error al obtener pedidos", new List<string> { ex.Message });
        }
    }
    public async Task<Result<PedidoDto>> ActualizarEstadoPedido(int id, ActualizarEstadoPedidoDto dto)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(id) ?? throw new Exception("Pedido no encontrado");

            // Validar transiciones de estado permitidas
            if (!Enum.IsDefined(typeof(EstadoPedido), dto.NuevoEstado))
            {
                var estadosPermitidos = Enum.GetNames<EstadoPedido>();
                throw new Exception(
                    $"Estado no válido. Estados permitidos: {string.Join(", ", estadosPermitidos)}");
            }

            // No permitir cambios si ya está entregado o cancelado
            if (pedido.Estado == EstadoPedido.Entregado || pedido.Estado == EstadoPedido.Cancelado)
            {
                throw new Exception(
                    $"No se puede cambiar el estado de un pedido {pedido.Estado}");
            }

            var estadoAnterior = pedido.Estado;
            pedido.Estado = (EstadoPedido)dto.NuevoEstado;
            pedido.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);
            await _unitOfWork.SaveChangesAsync();

            // Crear notificación de cambio de estado
            var notificacion = new Notificacion
            {
                UsuarioId = pedido.UsuarioId,
                Tipo = TipoNotificacion.CambioEstado,
                Mensaje = $"Tu pedido #{pedido.Id} ha cambiado de estado: {estadoAnterior} → {dto.NuevoEstado}",
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Estado de pedido #{id} actualizado a {dto.NuevoEstado}");

            var pedidoDto = pedido.Adapt<PedidoDto>();
            return Result<PedidoDto>.SuccessResult(pedidoDto, "Estado actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar estado: {ex.Message}");
            return Result<PedidoDto>.FailureResult("Error al actualizar estado", [ex.Message]);
        }
    }
    public async Task<Result<PedidoDto>> AsignarRepartidor(int id, AsignarRepartirdoDto dto)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(id) ?? throw new Exception("Pedido no encontrado");

            // Validar que el repartidor existe
            var repartidor = await _unitOfWork.Repository<Usuario>().GetByIdAsync(dto.RepartidorId) ?? throw new Exception("Repartidor no encontrado");

            // Validar que el estado permite asignar repartidor
            if (pedido.Estado != EstadoPedido.EnPreparacion && pedido.Estado != EstadoPedido.Pendiente)
            {
                throw new Exception(
                    "Solo se pueden asignar repartidores a pedidos en estado Pendiente o Procesando");
            }

            pedido.RepartidorId = dto.RepartidorId;
            pedido.Estado = EstadoPedido.Enviado;
            pedido.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);
            await _unitOfWork.SaveChangesAsync();

            // Notificar al repartidor
            var notificacionRepartidor = new Notificacion
            {
                UsuarioId = dto.RepartidorId,
                Tipo = TipoNotificacion.PedidoAsignado,
                Mensaje = $"Se te ha asignado el pedido #{pedido.Id} para entregar",
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            // Notificar al cliente
            var notificacionCliente = new Notificacion
            {
                UsuarioId = pedido.UsuarioId,
                Tipo = TipoNotificacion.PedidoEnCamino,
                Mensaje = $"Tu pedido #{pedido.Id} está en ruta",
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddRangeAsync(
                [notificacionRepartidor, notificacionCliente]);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Repartidor {dto.RepartidorId} asignado a pedido #{id}");

            var pedidoDto = pedido.Adapt<PedidoDto>();
            return Result<PedidoDto>.SuccessResult(pedidoDto, "Repartidor asignado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al asignar repartidor: {ex.Message}");
            return Result<PedidoDto>.FailureResult("Error al asignar repartidor", [ex.Message]);
        }
    }
    public async Task<Result<List<PedidoDto>>> ObtenerPedidosPorEstado(string estado)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                throw new Exception("El estado es requerido");
            }

            var valorEntero = (int)Enum.Parse<EstadoPedido>(estado, true);

            var pedidos = await _unitOfWork.Repository<Pedido>()
                .FindAsync(p => p.Estado == (EstadoPedido)valorEntero);

            var pedidosDto = pedidos.Adapt<List<PedidoDto>>();

            _logger.LogInformation($"Se obtuvieron {pedidos.Count()} pedidos con estado {estado}");
            return Result<List<PedidoDto>>.SuccessResult(pedidosDto);

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener pedidos por estado: {ex.Message}");
            return Result<List<PedidoDto>>.FailureResult(
                "Error al obtener pedidos", [ex.Message]);
        }
    }
    public async Task<Result<List<PedidoDto>>> ObtenerPedidosPorUsuario(int usuarioId)
    {
        try
        {
            // Verificar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId) ?? throw new Exception("Usuario no encontrado");
            var pedidos = await _unitOfWork.Repository<Pedido>()
                .FindAsync(p => p.UsuarioId == usuarioId);

            var pedidosDto = pedidos.Adapt<List<PedidoDto>>();

            _logger.LogInformation($"Se obtuvieron {pedidos.Count()} pedidos del usuario {usuarioId}");
            return Result<List<PedidoDto>>.SuccessResult(pedidosDto);
        }
        catch (Exception ex)
        {

            _logger.LogError($"Error al obtener pedidos por usuario: {ex.Message}");
            return Result<List<PedidoDto>>.FailureResult(
                "Error al obtener pedidos", [ex.Message]);
        }
    }
    public async Task<Result<DetallePedidoDto>> AgregarDetallePedido(int pedidoId, CrearDetallePedidoDto dto)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(pedidoId) ?? throw new Exception("Pedido no encontrado");

            // Solo permitir agregar detalles si el pedido está en Pendiente
            if (pedido.Estado != EstadoPedido.Pendiente)
            {
                throw new Exception(
                    "Solo se pueden agregar productos a pedidos en estado Pendiente");
            }

            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(dto.ProductoId) ?? throw new Exception("Producto no encontrado");
            if (producto.Stock < dto.Cantidad)
            {
                throw new Exception(
                    $"Stock insuficiente. Disponible: {producto.Stock}");
            }

            var detallePedido = new DetallePedido
            {
                PedidoId = pedidoId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = producto.Precio * dto.Cantidad
            };

            await _unitOfWork.Repository<DetallePedido>().AddAsync(detallePedido);

            // Actualizar stock
            producto.Stock -= (int)dto.Cantidad;
            await _unitOfWork.Repository<Producto>().UpdateAsync(producto);

            // Actualizar total del pedido
            pedido.Total += detallePedido.Subtotal;
            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Detalle agregado al pedido #{pedidoId}");

            var detallePedidoDto = detallePedido.Adapt<DetallePedidoDto>();
            return Result<DetallePedidoDto>.SuccessResult(detallePedidoDto, "Producto agregado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al agregar detalle: {ex.Message}");
            return Result<DetallePedidoDto>.FailureResult("Error al agregar producto", [ex.Message]);
        }
    }
    public async Task<Result<bool>> RemoverDetallePedido(int detallePedidoId)
    {
        try
        {
            var detalle = await _unitOfWork.Repository<DetallePedido>().GetByIdAsync(detallePedidoId) ?? throw new Exception("Detalle de pedido no encontrado");
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(detalle.PedidoId);

            if (pedido == null)
            {
                throw new Exception("Pedido no encontrado");
            }

            // Solo permitir remover detalles si el pedido está en Pendiente
            if (pedido.Estado != EstadoPedido.Pendiente)
            {
                throw new Exception(
                    "Solo se pueden remover productos de pedidos en estado Pendiente");
            }

            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(detalle.ProductoId);

            // Revertir stock
            if (producto != null)
            {
                producto.Stock += (int)detalle.Cantidad;
                await _unitOfWork.Repository<Producto>().UpdateAsync(producto);
            }

            // Actualizar total del pedido
            pedido.Total -= detalle.Subtotal;
            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);

            await _unitOfWork.Repository<DetallePedido>().DeleteAsync(detalle);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Detalle removido del pedido #{detalle.PedidoId}");
            return Result<bool>.SuccessResult(true, "Producto removido exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al remover detalle: {ex.Message}");
            return Result<bool>.FailureResult("Error al remover producto", [ex.Message]);
        }
    }
    public async Task<Result<PedidoDto>> CancelarPedido(int id)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(id) ?? throw new Exception("Pedido no encontrado");

            // Validar estado
            if (pedido.Estado == EstadoPedido.Entregado)
            {
                throw new Exception("No se puede cancelar un pedido ya entregado");
            }

            if (pedido.Estado == EstadoPedido.Cancelado)
            {
                throw new Exception("El pedido ya está cancelado");
            }

            // Iniciar transacción para revertir stock
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Obtener detalles del pedido
                var detalles = await _unitOfWork.Repository<DetallePedido>()
                    .FindAsync(d => d.PedidoId == id);

                // Revertir stock para cada producto
                foreach (var detalle in detalles)
                {
                    var producto = await _unitOfWork.Repository<Producto>()
                        .GetByIdAsync(detalle.ProductoId);

                    if (producto != null)
                    {
                        producto.Stock += (int)detalle.Cantidad;
                        await _unitOfWork.Repository<Producto>().UpdateAsync(producto);

                        // Registrar movimiento de inventario (Entrada)
                        var movimiento = new MovimientoInventario
                        {
                            ProductoId = detalle.ProductoId,
                            TipoMovimiento = TipoMovimiento.Entrada,
                            Cantidad = detalle.Cantidad,
                            Motivo = $"Cancelación de Pedido #{pedido.Id}",
                            FechaModificacion = DateTime.UtcNow
                        };

                        await _unitOfWork.Repository<MovimientoInventario>().AddAsync(movimiento);
                    }
                }

                // Cambiar estado del pedido
                pedido.Estado = EstadoPedido.Cancelado;
                pedido.FechaModificacion = DateTime.UtcNow;
                await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);

                // Notificar al usuario
                var notificacion = new Notificacion
                {
                    UsuarioId = pedido.UsuarioId,
                    Tipo = TipoNotificacion.PedidoCancelado,
                    Mensaje = $"Tu pedido #{pedido.Id} ha sido cancelado",
                    FechaCreacion = DateTime.UtcNow,
                    Leida = false
                };

                await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Pedido #{id} cancelado exitosamente");
                return Result<PedidoDto>.SuccessResult(pedido.Adapt<PedidoDto>(), "Pedido cancelado exitosamente");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cancelar pedido: {ex.Message}");
            return Result<PedidoDto>.FailureResult("Error al cancelar pedido", [ex.Message]);
        }
    }
    public async Task<Result> ActualizarAsync(PedidoDto dto)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(dto.Id) ?? throw new Exception("Pedido no encontrado");

            // Solo permitir actualizar pedidos en Pendiente
            if (pedido.Estado != EstadoPedido.Pendiente)
            {
                throw new Exception(
                    "Solo se pueden actualizar pedidos en estado Pendiente");
            }

            // Mapear cambios con Mapster (excepto Id y Total que se calculan)
            dto.Adapt(pedido);
            pedido.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Repository<Pedido>().UpdateAsync(pedido);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Pedido #{dto.Id} actualizado exitosamente");
            return Result.SuccessResult("Pedido actualizado exitosamente");
        }
        catch (Exception ex)
        {

            _logger.LogError($"Error al actualizar pedido: {ex.Message}");
            return Result.FailureResult("Error al actualizar pedido", [ex.Message]);
        }
    }

    public async Task<Result> EliminarAsync(int id)
    {
        try
        {
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(id) ?? throw new Exception("Pedido no encontrado");

            // Solo permitir eliminar pedidos en estado Pendiente o Cancelado
            if (pedido.Estado != EstadoPedido.Pendiente && pedido.Estado != EstadoPedido.Cancelado)
            {
                throw new Exception(
                    $"Solo se pueden eliminar pedidos en estado Pendiente o Cancelado. Estado actual: {pedido.Estado}");
            }

            // Iniciar transacción para revertir stock si es necesario
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Si el pedido no está cancelado, revertir stock
                if (pedido.Estado != EstadoPedido.Cancelado)
                {
                    var detalles = await _unitOfWork.Repository<DetallePedido>()
                        .FindAsync(d => d.PedidoId == id);

                    foreach (var detalle in detalles)
                    {
                        var producto = await _unitOfWork.Repository<Producto>()
                            .GetByIdAsync(detalle.ProductoId);

                        if (producto != null)
                        {
                            producto.Stock += (int)detalle.Cantidad;
                            await _unitOfWork.Repository<Producto>().UpdateAsync(producto);

                            // Registrar movimiento de inventario (Entrada)
                            var movimiento = new MovimientoInventario
                            {
                                ProductoId = detalle.ProductoId,
                                TipoMovimiento = TipoMovimiento.Entrada,
                                Cantidad = detalle.Cantidad,
                                Motivo = $"Eliminación de Pedido #{pedido.Id}",
                                FechaModificacion = DateTime.UtcNow
                            };

                            await _unitOfWork.Repository<MovimientoInventario>().AddAsync(movimiento);
                        }
                    }

                    // Eliminar detalles del pedido
                    await _unitOfWork.Repository<DetallePedido>().DeleteRangeAsync(detalles);
                }

                // Eliminar notificaciones asociadas
                var notificaciones = await _unitOfWork.Repository<Notificacion>()
                    .FindAsync(n => n.Mensaje.Contains($"#{pedido.Id}"));

                if (notificaciones.Any())
                {
                    await _unitOfWork.Repository<Notificacion>().DeleteRangeAsync(notificaciones);
                }

                // Eliminar el pedido
                await _unitOfWork.Repository<Pedido>().DeleteAsync(pedido);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Pedido #{id} eliminado exitosamente");
                return Result.SuccessResult("Pedido eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante la eliminación del pedido #{id}, realizando rollback");
                await _unitOfWork.RollbackTransactionAsync();
                return Result.FailureResult("Error al eliminar pedido", [ex.Message]);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar pedido: {ex.Message}");
            return Result.FailureResult("Error al eliminar pedido", [ex.Message]);
        }
    }

}
