namespace Almacen.Saas.Application.DTOs.Notificacion;
public class CrearNotificacionDto
{
    public int UsuarioId { get; set; }
    public int TipoNotificacion { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Referencia { get; set; }
}
