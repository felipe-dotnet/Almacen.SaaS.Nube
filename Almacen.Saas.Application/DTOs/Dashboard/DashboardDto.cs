namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public EstadisticasVentasDto Ventas { get; set; } = new();
        public EstadisticasPedidosDto Pedidos { get; set; } = new();
        public EstadisticasInventarioDto Inventario { get; set; } = new();
        public List<ProductoMasVendidoDto> ProductosMasVendidos { get; set; } = new();
        public List<AlertaDto> Alertas { get; set; } = new();
    }
}
