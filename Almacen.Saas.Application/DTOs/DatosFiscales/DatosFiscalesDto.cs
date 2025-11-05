namespace Almacen.Saas.Application.DTOs.DatosFiscales;
public class DatosFiscalesDto
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumeroExterior { get; set; } = string.Empty;
    public string NumeroInterior { get; set; } = string.Empty;
    public string Referencia { get; set; }=string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public string TipoPersona { get; set; }=string.Empty;
    public int RegimenFiscalId { get; set; }
}
