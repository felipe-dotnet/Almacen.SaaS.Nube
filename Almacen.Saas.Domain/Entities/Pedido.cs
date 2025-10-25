using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;
public class Pedido: BaseEntity,IAuditableEntity
{
    public string NumeroPedido { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime FechaPedido { get; set; }
    public EstadoPedido Estado { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal CostoEnvio { get; set; }
    public decimal Total { get; set; }

    // Dirección de envío (puede ser diferente a la del usuario)
    public string DireccionEnvio { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? NotasInternas { get; set; }

    // Asignación a repartidor
    public int? RepartidorId { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaEntrega { get; set; }

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public Usuario? Repartidor { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = [];
    public Factura? Factura { get; set; }
}