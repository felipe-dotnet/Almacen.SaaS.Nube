namespace Almacen.Saas.Domain.Common;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; }=true;
}
