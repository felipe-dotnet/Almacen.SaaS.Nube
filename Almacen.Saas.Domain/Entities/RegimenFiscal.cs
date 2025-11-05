using Almacen.Saas.Domain.Common;

namespace Almacen.Saas.Domain.Entities;
public class RegimenFiscal:BaseEntity
{
    public string Codigo { get; set; }=string.Empty;
    public string Descripcion { get; set; }= string.Empty;    
}
