using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;
public class Notificacion: BaseEntity
{
    public int UsuarioId { get; set; }
    public TipoNotificacion Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; } = false;
    public DateTime? FechaLectura { get; set; }
    public string? Referencia { get; set; }

    // Navegación
    public Usuario Usuario { get; set; } = null!;
}