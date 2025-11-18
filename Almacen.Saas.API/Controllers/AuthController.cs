using Almacen.Saas.Application.DTO.Authentication;
using Almacen.Saas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var result = await _authenticationService.AuthenticateAsync(loginRequest);

            if (result == null)
                return BadRequest("Email o contraseña inválidos");

            return Ok(result, "Autenticación exitosa");
        }
    }
}
