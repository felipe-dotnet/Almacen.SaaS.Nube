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
        public int PedidosEnRuta { get; set; }
        public int TasaEntrega { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal VentaPromedio { get; set; }
        public double TiempoPromedio { get; set; }
        public string Periodo { get; set; }=string.Empty;
    }
}