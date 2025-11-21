using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;
public class Usuario : BaseEntity, IAuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }

    // Dirección
    public string Calle { get; set; } = string.Empty;
    public string? NumeroExterior { get; set; }
    public string? NumeroInterior { get; set; }
    public string Colonia { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;

    // Auditoría
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; }

    // Navegación
    public ICollection<Pedido> Pedidos { get; set; } = [];
    public ICollection<Notificacion> Notificaciones { get; set; } =[];
    public ICollection<DatosFiscales> DatosFiscales { get; set; } = [];
    public ICollection<RefreshToken> RefresTokens { get; set; } = [];
    public ICollection<PasswordResetToken> PasswordResetToken { get; set; } = [];
}
