using System.ComponentModel.DataAnnotations;

namespace Almacen.Saas.Application.Services.Authentication;

public class LoginRequest
{
    [Required(ErrorMessage = "El Email es requerido")]
    [EmailAddress(ErrorMessage = "El Email no es válido")]
    [MaxLength(256, ErrorMessage = "El Email no puede exceder los 256 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}
