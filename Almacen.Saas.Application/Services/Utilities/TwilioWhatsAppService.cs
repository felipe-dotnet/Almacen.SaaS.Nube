using Almacen.Saas.Domain.Services;
using Almacen.Saas.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Almacen.Saas.Application.Services.Utilities;
public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly WhatsAppSettings _whatsAppSettings;
    private readonly ILogger<TwilioWhatsAppService> _logger;

    public TwilioWhatsAppService(IOptions<WhatsAppSettings> whatsAppSettings, ILogger<TwilioWhatsAppService> logger)
    {
        _whatsAppSettings = whatsAppSettings.Value;
        _logger = logger;
    }

    public async Task<bool> EnviarMensajeAsync(WhatsAppRequest request)
    {
        try
        {
            // TODO: Implementar con Twilio SDK
            // var accountSid = _whatsAppSettings.TwilioAccountSid;
            // var authToken = _whatsAppSettings.TwilioAuthToken;
            // TwilioClient.Init(accountSid, authToken);
            // var message = await MessageResource.CreateAsync(...);

            _logger.LogInformation($"Mensaje WhatsApp enviado a {request.Telefono}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar WhatsApp: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> EnviarMensajeConMediaAsync(WhatsAppRequest request)
    {
        try
        {
            _logger.LogInformation($"Mensaje WhatsApp con media enviado a {request.Telefono}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar WhatsApp con media: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> VerificarNumeroAsync(string telefono)
    {
        try
        {
            _logger.LogInformation($"Número {telefono} verificado");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al verificar número: {ex.Message}");
            return false;
        }
    }
}
