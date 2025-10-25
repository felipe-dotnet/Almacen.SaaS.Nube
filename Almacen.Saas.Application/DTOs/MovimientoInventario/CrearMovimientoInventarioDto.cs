namespace Almacen.Saas.Application.DTOs.MovimientoInventario;

public class CrearMovimientoInventarioDto
{
    public int ProductoId { get; set; }
    public int TipoMovimiento { get; set; } // Enum como int
    public decimal Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Referencia { get; set; }
}
