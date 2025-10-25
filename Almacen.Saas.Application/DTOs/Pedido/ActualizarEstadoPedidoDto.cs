namespace Almacen.Saas.Application.DTOs.Pedido
{
    public class ActualizarEstadoPedidoDto
    {
        public int PedidoId { get; set; }
        public int NuevoEstado { get; set; }
        public string? NotasInternas { get; set; }
    }
}
