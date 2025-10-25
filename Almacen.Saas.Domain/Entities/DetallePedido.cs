using Almacen.Saas.Domain.Common;

namespace Almacen.Saas.Domain.Entities;

public class DetallePedido:BaseEntity
{
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    // Navegación
    public Pedido Pedido { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}