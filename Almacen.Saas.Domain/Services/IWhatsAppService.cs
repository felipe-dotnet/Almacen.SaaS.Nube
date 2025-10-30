namespace Almacen.Saas.Domain.Services;
public interface IWhatsAppService
{
    Task<bool> EnviarMensajeAsync(WhatsAppRequest request);
    Task<bool> EnviarMensajeConMediaAsync(WhatsAppRequest request);
    Task<bool> VerificarNumeroAsync(string telefono);
}
public class WhatsAppRequest
{
    public string Telefono { get; set; }=string.Empty;
    public string Mensaje { get; set; }=string.Empty;
    public string MediaUrl { get; set; } = string.Empty;         
}
