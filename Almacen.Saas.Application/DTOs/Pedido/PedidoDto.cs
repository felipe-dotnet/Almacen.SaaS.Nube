namespace Almacen.Saas.Application.DTOs.Pedido;
public class PedidoDto
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime FechaPedido { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal CostoEnvio { get; set; }
    public decimal Total { get; set; }
    public string DireccionEnvio { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public List<DetallePedidoDto> Detalles { get; set; } = new();
}
