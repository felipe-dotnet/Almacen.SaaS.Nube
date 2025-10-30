namespace Almacen.Saas.Domain.Settings;
public class EmailSettings
{
    public string SmtpHost { get; set; }= string.Empty;
    public int SmtpPort { get; set; }
    public string Origen { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
