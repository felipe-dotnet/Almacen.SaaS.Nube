namespace Almacen.Saas.Application.DTOs.Dashboard
{
    public class AlertaDto
    {
        public string Tipo { get; set; }=string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Severidad { get; set; }= string.Empty;
        public DateTime FechaAlerta { get; set; }
    }
}