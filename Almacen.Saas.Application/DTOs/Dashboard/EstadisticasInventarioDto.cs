namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class EstadisticasInventarioDto
    {
        public int TotalProductos { get; set; }
        public int StockTotal { get; set; }
        public int ProductosStockBajo { get; set; }
        public int ProductosSinStock { get; set; }
        public decimal ValorInventario { get; set; }
        public decimal StockPromedio { get; set; }
    }
}