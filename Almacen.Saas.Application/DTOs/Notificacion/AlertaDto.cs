namespace Almacen.Saas.Application.DTOs.Notificacion;
public class AlertaDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty; // Info, Warning, Error
    public DateTime Fecha { get; set; }
}
