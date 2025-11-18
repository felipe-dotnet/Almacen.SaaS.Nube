namespace Almacen.Saas.Domain.Settings;

/// <summary>
/// Configuración de JWT para autenticación
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Clave secreta para firmar el token
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emisor del token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiencia del token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Expiración del token en minutos
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Expiración del refresh token en días
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
