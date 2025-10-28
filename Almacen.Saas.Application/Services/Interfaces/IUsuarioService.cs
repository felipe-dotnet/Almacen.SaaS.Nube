using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Usuario;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<Result<IEnumerable<UsuarioDto>>> ObtenerTodosAsync();
    Task<Result<UsuarioDto>> ObtenerPorIdAsync(int id);
    Task<Result<UsuarioDto>> ObtenerPorEmailAsync(string email);
    Task<Result<UsuarioDto>> CrearAsync(CrearUsuarioDto dto);
    Task<Result<UsuarioDto>> ActualizarAsync(ActualizarUsuarioDto dto);
    Task<Result> EliminarAsync(int id);
    Task<Result> CambiarPasswordAsync(CambiarPasswordDto dto);
    Task<Result<IEnumerable<UsuarioDto>>> ObtenerPorRolAsync(int rol);
    //Task<Result<IEnumerable<UsuarioDto>>> ObtenerClientesAsync();
    //Task<Result<IEnumerable<UsuarioDto>>> ObtenerRepartidoresAsync();
}
