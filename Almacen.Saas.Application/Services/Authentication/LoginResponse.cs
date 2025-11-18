namespace Almacen.Saas.Application.Services.Authentication
{
    /// <summary>
    /// DTO para respuesta exitosa de login
    /// </summary>
    public class LoginResponse
    {
        public int UsuarioId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public string Rol { get; set; } = string.Empty;
    }
}
