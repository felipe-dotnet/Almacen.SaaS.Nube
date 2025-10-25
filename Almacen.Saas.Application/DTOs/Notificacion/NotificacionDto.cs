namespace Almacen.Saas.Application.DTOs.Notificacion;

public class NotificacionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public DateTime? FechaLectura { get; set; }
    public string? Referencia { get; set; }
    public DateTime FechaCreacion { get; set; }
}
