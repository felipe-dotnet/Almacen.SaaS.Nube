using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Pedido;

namespace Almacen.Saas.Application.Services.Interfaces;
public interface IPedidoService
{
    Task<Result<int>> CrearAsync(CrearPedidoDto dto);
    Task<Result<PedidoDto>> ObtenerPorIdAsync(int id);
    Task<Result<PagedResultDto<PedidoDto>>> ObtenerTodosAsync(int page, int pageSize);
    Task<Result<List<PedidoDto>>> ObtenerPedidosPorEstado(string estado);
    Task<Result<List<PedidoDto>>> ObtenerPedidosPorUsuario(int usuarioId);
    Task<Result> ActualizarAsync(PedidoDto dto);
    Task<Result<PedidoDto>> ActualizarEstadoPedido(int id, ActualizarEstadoPedidoDto dto);
    Task<Result> EliminarAsync(int id);
    // Métodos específicos pedidos
    Task<Result<PedidoDto>> AsignarRepartidor(int id, AsignarRepartirdoDto dto);
    Task<Result<PedidoDto>> CancelarPedido(int id);
    Task<Result<DetallePedidoDto>> AgregarDetallePedido(int pedidoId, CrearDetallePedidoDto dto);
    Task<Result<bool>> RemoverDetallePedido(int detallePedidoId);
}
