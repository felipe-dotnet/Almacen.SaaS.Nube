using Almacen.Saas.Application.DTOs.Usuario;
using Almacen.Saas.Application.Services.Authentication;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Interfaces;
using Almacen.Saas.Domain.Services;
using Almacen.Saas.Domain.Settings;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Almacen.Saas.Application.Services.Implementations.Authentication;

/// <summary>
/// Implementación del servicio de autenticación con JWT
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthenticationService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<LoginResponse?> AuthenticateAsync(LoginRequest loginRequest)
    {
        if (loginRequest == null)
        {
            throw new ArgumentNullException(nameof(loginRequest));
        }

        _logger.LogInformation("Intento de autenticación para: {Email}", loginRequest.Email);

        try
        {
            // Buscar usuario por email
            var usuarioBase = await _unitOfWork.Repository<Usuario>().GetAsync(x=>x.Email==loginRequest.Email);

            var usuario=usuarioBase.Adapt<UsuarioDto>();

            if (usuario == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Email}", loginRequest.Email);
                return null;
            }

            // Verificar contraseña
            if (!_passwordHasher.VerifyPassword(usuario.PasswordHashed, loginRequest.Password))
            {
                _logger.LogWarning("Contraseña incorrecta para usuario: {Email}", loginRequest.Email);
                return null;
            }

            // Generar JWT
            string accessToken;
            if (usuarioBase == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Email}", loginRequest.Email);
                return null;
            }
            accessToken = GenerateAccessToken(usuarioBase);
            var refreshToken = Guid.NewGuid().ToString("N");

            // Guardar refresh token (opcional - depende de tu BD)
            // await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken { UsuarioId = usuario.Id, Token = refreshToken });
            // await _unitOfWork.SaveChangesAsync();

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            _logger.LogInformation("Autenticación exitosa para usuario: {Email}", loginRequest.Email);

            return new LoginResponse
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Rol = usuario.Rol.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante autenticación de usuario: {Email}", loginRequest.Email);
            throw;
        }
    }

    public async Task<Usuario?> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var usuarioId))
                return null;

            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId);
            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validando token");
            return null;
        }
    }

    public async Task<string> GenerateRefreshTokenAsync(int usuarioId)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId);
        return usuario == null
            ? throw new InvalidOperationException($"Usuario con ID {usuarioId} no encontrado")
            : Guid.NewGuid().ToString("N");
    }

    private string GenerateAccessToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
