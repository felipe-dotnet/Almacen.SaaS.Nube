using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;
public class Producto:BaseEntity,IAuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public UnidadMedida UnidadMedida { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 10;
    public bool Disponible { get; set; } = true;

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
    public ICollection<DetallePedido> DetallesPedido { get; set; } = [];
    public ICollection<MovimientoInventario> Movimientos { get; set; } = [];
}
