using Almacen.Saas.Domain.Common;
using Almacen.Saas.Domain.Enums;

namespace Almacen.Saas.Domain.Entities;
public class DatosFiscales:BaseEntity,IAuditableEntity
{
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; }=string.Empty;
    public TipoPersonaFiscal TipoPersonaFiscal { get; set; }
    public string Calle { get; set; }= string.Empty;
    public string NumExterior { get; set; } = string.Empty;
    public string NumInt { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public int RegimenFiscalId { get; set; }
    public Usuario Usuario { get; set; }=new Usuario();
    public string CreadoPor { get; set; } = string.Empty;
    public string? ModificadoPor { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public RegimenFiscal RegimenFiscal { get; set; } = new RegimenFiscal();
}
