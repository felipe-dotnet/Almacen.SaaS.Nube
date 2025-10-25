using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Application.DTOs.Producto;
public class CrearProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public int UnidadMedida { get; set; } // Enum como int
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 10;
}
