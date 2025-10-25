namespace Almacen.Saas.Application.DTOs.Dashboard;

public class EstadisticasVentasDto
{
    public decimal VentasHoy { get; set; }
    public decimal VentasSemana { get; set; }
    public decimal VentasMes { get; set; }
    public decimal VentasAnio { get; set; }
    public decimal CrecimientoMensual { get; set; }
}