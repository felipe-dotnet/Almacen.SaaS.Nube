using Almacen.Saas.Domain.Services;
using Almacen.Saas.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Almacen.Saas.Application.Services.Utilities;
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

        #region Constructor
    public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }
    #endregion
    public async Task<bool> EnviarEmailAsync(EmailRequest request)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            client.Credentials = new NetworkCredential(_emailSettings.Origen, _emailSettings.Contrasena);
            client.EnableSsl = _emailSettings.EnableSsl;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Origen),
                Subject = request.Asunto,
                Body = request.Cuerpo,
                IsBodyHtml = request.EsHtml
            };

            mailMessage.To.Add(request.Destinatario);

            foreach (var archivo in request.Archivos)
            {
                mailMessage.Attachments.Add(new Attachment(archivo));
            }

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email enviado a {request.Destinatario}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EnviarEmailEnBackgroundAsync(EmailRequest request)
    {
        _ = Task.Run(() => EnviarEmailAsync(request));
        return await Task.FromResult(true);
    }

    public async Task<bool> EnviarEmailTemplateAsync(string destinatario, string template, Dictionary<string, string> variables)
    {
        try
        {
            var cuerpoTemplate = CargarTemplate(template);

            foreach (var variable in variables)
            {
                cuerpoTemplate = cuerpoTemplate.Replace($"{{{{{variable.Key}}}}}", variable.Value);
            }

            var request = new EmailRequest
            {
                Destinatario = destinatario,
                Asunto = variables.ContainsKey("Asunto") ? variables["Asunto"] : "Notificación",
                Cuerpo = cuerpoTemplate,
                EsHtml = true
            };

            return await EnviarEmailAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar email template: {ex.Message}");
            return false;
        }
    }
    private string CargarTemplate(string template)
    {
        return $"<html><body><p>{{Contenido}}</p></body></html>";
    }
}
