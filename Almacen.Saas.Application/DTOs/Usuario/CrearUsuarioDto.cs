﻿namespace Almacen.Saas.Application.DTOs.Usuario;
public class CrearUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Rol { get; set; } = 1; // Cliente por defecto

    // Dirección
    public string Calle { get; set; } = string.Empty;
    public string? NumeroExterior { get; set; }
    public string? NumeroInterior { get; set; }
    public string Colonia { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
}
