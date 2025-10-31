namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class ProductoMasVendidoDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal Precio { get; set; }
    }
}