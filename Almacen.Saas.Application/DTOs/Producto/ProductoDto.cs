using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Application.DTOs.Producto;
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public bool Disponible { get; set; }
    public bool BajoStock => Stock <= StockMinimo;
    public DateTime FechaCreacion { get; set; }
}
