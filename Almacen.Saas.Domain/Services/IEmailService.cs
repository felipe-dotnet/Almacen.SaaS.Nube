namespace Almacen.Saas.Domain.Services;
public interface IEmailService
{
    Task<bool> EnviarEmailAsync(EmailRequest request);
    Task<bool> EnviarEmailEnBackgroundAsync(EmailRequest request);
    Task<bool> EnviarEmailTemplateAsync(string destinatario, string template, Dictionary<string, string> variables);
}

public class EmailRequest
{
    public string Destinatario { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Cuerpo { get; set; } = string.Empty;
    public bool EsHtml { get; set; } = true;
    public List<string> Archivos { get; set; } = [];
}
