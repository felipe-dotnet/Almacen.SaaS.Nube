namespace Almacen.Saas.Application.DTOs.Factura;
public class FacturaDto
{
    public int Id { get; set; }
    public string FolioFiscal { get; set; } = string.Empty;
    public int PedidoId { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
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

    public DateTime FechaCreacion { get; set; }
}
