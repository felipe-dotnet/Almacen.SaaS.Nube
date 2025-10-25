using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;

public class MovimientoInventario:BaseEntity,IAuditableEntity
{
    public int ProductoId { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    public decimal Cantidad { get; set; }
    public int StockAnterior { get; set; }
    public int StockNuevo { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Referencia { get; set; }

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
    public Producto Producto { get; set; } = null!;
}