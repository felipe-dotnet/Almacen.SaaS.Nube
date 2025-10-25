namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class EstadisticasPedidosDto
    {
        public int TotalPedidos { get; set; }
        public int PedidosPendientes { get; set; }
        public int PedidosEnPreparacion { get; set; }
        public int PedidosEnviados { get; set; }
        public int PedidosEntregados { get; set; }
        public int PedidosCancelados { get; set; }
    }
}