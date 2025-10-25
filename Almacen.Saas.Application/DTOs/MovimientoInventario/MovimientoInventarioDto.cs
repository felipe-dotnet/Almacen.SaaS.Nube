namespace Almacen.Saas.Application.DTOs.MovimientoInventario;
public class MovimientoInventarioDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public int StockAnterior { get; set; }
    public int StockNuevo { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public string CreadoPor { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
