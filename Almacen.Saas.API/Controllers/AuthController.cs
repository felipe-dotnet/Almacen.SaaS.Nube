using Almacen.Saas.Application.Services.Authentication;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Almacen.Saas.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPasswordRecoveryService _passwordRecoveryService;

        public AuthController(IAuthenticationService authenticationService, IPasswordRecoveryService passwordRecoveryService)
        {
            _authenticationService = authenticationService;
            _passwordRecoveryService = passwordRecoveryService;
        }

        /// <summary>
        /// Autentica un usuario y retorna un JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Application.Services.Authentication.LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos", ["Modelo no valido"]);

            var result = await _authenticationService.AuthenticateAsync(loginRequest);

            if (result == null)
                return BadRequest("Email o contraseña inválidos", ["Email o contraseña inválidos"]);

            return Ok(result, "Autenticación exitosa");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
            if (response == null)
                return Unauthorized();
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var resultado = await _authenticationService.LogoutAsync(request.RefreshToken);
            if (!resultado)
                return BadRequest("Logout fallido.");
            return Ok("Sesión cerrada correctamente.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            await _passwordRecoveryService.SolicitarRecuperacionAsync(req.Email);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            var ok = await _passwordRecoveryService.CambiarPasswordAsync(req.ResetCode, req.NewPassword);
            if (!ok) return BadRequest("El token es inválido o ya expiró.");
            return Ok();
        }
    }
}
