using Almacen.Saas.Domain.Common;

namespace Almacen.Saas.Domain.Entities;

public class Factura:BaseEntity,IAuditableEntity
{
    public string FolioFiscal { get; set; } = string.Empty;
    public int PedidoId { get; set; }
    public DateTime FechaEmision { get; set; }

    // Datos del receptor
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string DireccionFiscal { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Montos
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }

    // Archivos
    public string? XmlUrl { get; set; }
    public string? PdfUrl { get; set; }

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
    public Pedido Pedido { get; set; } = null!;
}