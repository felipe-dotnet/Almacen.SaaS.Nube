namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class EstadisticasInventarioDto
    {
        public int TotalProductos { get; set; }
        public int ProductosDisponibles { get; set; }
        public int ProductosBajoStock { get; set; }
        public int ProductosAgotados { get; set; }
    }
}