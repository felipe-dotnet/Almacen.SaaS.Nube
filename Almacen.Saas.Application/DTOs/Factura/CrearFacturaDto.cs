namespace Almacen.Saas.Application.DTOs.Factura;
public class CrearFacturaDto
{
    public int PedidoId { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string DireccionFiscal { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
