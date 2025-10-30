namespace Almacen.Saas.Domain.Services;
public interface INotificacionChannelService
{
    Task<bool> NotificarPorEmailAsync(string email, string asunto, string mensaje);
    Task<bool> NotificarPorWhatsAppAsync(string telefono, string mensaje);
    Task<Dictionary<string, bool>> NotificarPorMultiplesCanalesAsync(
        string email,
        string telefono,
        string asunto,
        string mensaje,
        params string[] canales);
}
