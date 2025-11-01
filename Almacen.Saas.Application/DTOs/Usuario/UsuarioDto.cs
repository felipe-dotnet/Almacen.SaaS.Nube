using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Application.DTOs.Usuario;
public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;

    // Dirección
    public string Calle { get; set; } = string.Empty;
    public string? NumeroExterior { get; set; }
    public string? NumeroInterior { get; set; }
    public string Colonia { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;

    // Datos Fiscales (opcional para clientes que requieran factura)
    public string? RFC { get; set; }
    public string? RazonSocial { get; set; }
    public string? DireccionFiscal { get; set; }

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
   // public ICollection<Pedido> Pedidos { get; set; } = [];
   // public ICollection<Notificacion> Notificaciones { get; set; } = [];
}
