namespace Almacen.Saas.Application.DTOs.Usuario;

public class CambiarPasswordDto
{
    public int UsuarioId { get; set; }
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNuevo { get; set; } = string.Empty;
    public string ConfirmarPassword { get; set; } = string.Empty;
}
