namespace Almacen.Saas.Domain.Common;

public interface IAuditableEntity
{
    string CreadoPor { get; set; }
    DateTime FechaCreacion { get; set; }
    string? ModificadoPor { get; set; }
    DateTime? FechaModificacion { get; set; }
}
