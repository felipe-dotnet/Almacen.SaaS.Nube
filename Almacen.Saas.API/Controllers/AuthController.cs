using Almacen.Saas.Application.Services.Authentication;
using Almacen.Saas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almacen.Saas.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Autentica un usuario y retorna un JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos",["Modelo no valido"]);

            var result = await _authenticationService.AuthenticateAsync(loginRequest);

            if (result == null)
                return BadRequest("Email o contraseña inválidos",["Email o contraseña inválidos"]);

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
    }
}
