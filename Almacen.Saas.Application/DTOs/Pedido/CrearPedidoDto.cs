namespace Almacen.Saas.Application.DTOs.Pedido;
public class CrearPedidoDto
{
    public int UsuarioId { get; set; }
    public string DireccionEnvio { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public decimal CostoEnvio { get; set; } = 0;
    public List<CrearDetallePedidoDto> Detalles { get; set; } = new();
}
