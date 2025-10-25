namespace Almacen.Saas.Application.DTOs.Pedido;
public class DetallePedidoDto
{
    public int Id { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
