using Almacen.Saas.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Utilities;
public class NotificacionChannelService: INotificacionChannelService
{
    private readonly IEmailService _emailService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly ILogger<NotificacionChannelService> _logger;

    public NotificacionChannelService(
        IEmailService emailService,
        IWhatsAppService whatsAppService,
        ILogger<NotificacionChannelService> logger)
    {
        _emailService = emailService;
        _whatsAppService = whatsAppService;
        _logger = logger;
    }

    public async Task<bool> NotificarPorEmailAsync(string email, string asunto, string mensaje)
    {
        var request = new EmailRequest
        {
            Destinatario = email,
            Asunto = asunto,
            Cuerpo = mensaje,
            EsHtml = true
        };

        return await _emailService.EnviarEmailAsync(request);
    }

    public async Task<bool> NotificarPorWhatsAppAsync(string telefono, string mensaje)
    {
        var request = new WhatsAppRequest
        {
            Telefono = telefono,
            Mensaje = mensaje
        };

        return await _whatsAppService.EnviarMensajeAsync(request);
    }

    public async Task<Dictionary<string, bool>> NotificarPorMultiplesCanalesAsync(
        string email,
        string telefono,
        string asunto,
        string mensaje,
        params string[] canales)
    {
        var resultados = new Dictionary<string, bool>();

        foreach (var canal in canales)
        {
            try
            {
                switch (canal.ToLower())
                {
                    case "email":
                        resultados["Email"] = await NotificarPorEmailAsync(email, asunto, mensaje);
                        break;
                    case "whatsapp":
                        resultados["WhatsApp"] = await NotificarPorWhatsAppAsync(telefono, mensaje);
                        break;
                    default:
                        _logger.LogWarning($"Canal desconocido: {canal}");
                        resultados[canal] = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al notificar por {canal}: {ex.Message}");
                resultados[canal] = false;
            }
        }

        return resultados;
    }
}
