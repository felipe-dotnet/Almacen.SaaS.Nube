using Almacen.Saas.Application.Services.Authentication;
using Almacen.Saas.Domain.Entities;

namespace Almacen.Saas.Application.Services.Interfaces;

/// <summary>
/// Servicio de autenticación con JWT
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica un usuario con email y contraseña
    /// </summary>
    /// <param name="loginRequest">Credenciales del usuario</param>
    /// <returns>Token JWT y datos del usuario</returns>
    Task<LoginResponse?> AuthenticateAsync(LoginRequest loginRequest);

    /// <summary>
    /// Valida un token JWT y retorna los datos del usuario
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Datos del usuario si el token es válido</returns>
    Task<Usuario?> ValidateTokenAsync(string token);

    /// <summary>
    /// Genera un refresh token
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Nuevo refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(int usuarioId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
}
