namespace Almacen.Saas.Application.DTOs.DatosFiscales;

public class ActualizarDatosFiscalesDto
{
    public int UsuarioId { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumeroExterior { get; set; } = string.Empty;
    public string NumeroInterior { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public int TipoPersona { get; set; } 
    public int RegimenFiscalId { get; set; }
}
