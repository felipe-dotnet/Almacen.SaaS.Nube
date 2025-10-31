using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Usuario;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Application.Validators.Usuario;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Interfaces;
using Almacen.Saas.Domain.Services;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;



namespace Almacen.Saas.Application.Services.Implementations;
public class UsuarioService : IUsuarioService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UsuarioService> _logger;
    public UsuarioService(IUnitOfWork unitOfWork, ILogger<UsuarioService> logger, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UsuarioDto>> CrearAsync(CrearUsuarioDto dto)
    {
        try
        {
            // 1. Validar
            var validator = new CrearUsuarioValidator();
            var validatorResult = validator.Validate(dto);

            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await Task.FromResult(Result<UsuarioDto>.FailureResult("Errores de validación", errors));
            }

            // 2. Verificar si el usuario ya existe
            var usuarioExistente = await _unitOfWork.Repository<Usuario>().ExistsAsync(u => u.Email == dto.Email);

            if (!usuarioExistente)
            {
                throw new Exception("El usuario con este email ya existe");
            }

            var usuario = dto.Adapt<Usuario>();
            usuario.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            await _unitOfWork.Repository<Usuario>().AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            var usuarioDto = usuario.Adapt<UsuarioDto>();
            return Result<UsuarioDto>.SuccessResult(usuarioDto, "Usuario creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener usuario por email: {Message}", ex.Message);
            return Result<UsuarioDto>.FailureResult("Error al obtener usuario", [ex.Message]);
        }
    }

    public async Task<Result<UsuarioDto>> ActualizarAsync(ActualizarUsuarioDto dto)
    {
        try
        {
            // 1. Validar
            var validator = new ActualizarUsuarioValidator();
            var validatorResult = validator.Validate(dto);

            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await Task.FromResult(Result<UsuarioDto>.FailureResult("Errores de validación", errors));
            }

            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(dto.Id);

            if (usuario == null)
            {
                return Result<UsuarioDto>.FailureResult("Usuario no encontrado", ["No se encontró un usuario con el ID proporcionado"]);
            }

            dto.Adapt(usuario);

            await _unitOfWork.Repository<Usuario>().UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            var usuarioDto = usuario.Adapt<UsuarioDto>();
            return Result<UsuarioDto>.SuccessResult(usuarioDto, "Usuario actualizado exitosamente");
        }
        catch (Exception ex)
        {

            _logger.LogError("Error al actualizar al usuario: {Message}", ex.Message);
            return Result<UsuarioDto>.FailureResult("Error al actualizar usuario", [ex.Message]);
        }

    }

    public async Task<Result> CambiarPasswordAsync(CambiarPasswordDto dto)
    {

        try
        {
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(dto.UsuarioId);
            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado", ["No se encontró un usuario con el ID proporcionado"]);
            }

            if (!_passwordHasher.VerifyPassword(usuario.PasswordHash, dto.PasswordActual))
            {
                return Result.FailureResult("Contraseña actual incorrecta", ["La contraseña actual no coincide"]);
            }

            if(dto.PasswordActual != dto.PasswordNuevo)
            {
                return Result.FailureResult("La nueva contraseña y la confirmación no coinciden", ["La nueva contraseña y la confirmación no coinciden"]);
            }

            usuario.PasswordHash = _passwordHasher.HashPassword(dto.PasswordNuevo);

            await _unitOfWork.Repository<Usuario>().UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return Result.SuccessResult("Contraseña cambiada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al actualizar al usuario: {Message}", ex.Message);
            return Result.FailureResult("Error al actualizar usuario", [ex.Message]);
        }

    }

    public async Task<Result> EliminarAsync(int id)
    {
        try
        {
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(id) ?? throw new Exception("Usuario no encontrado");
            await _unitOfWork.Repository<Usuario>().DeleteAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Usuario {id} eliminado exitosamente");
            return Result.SuccessResult("Usuario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar usuario: {ex.Message}");
            return Result.FailureResult("Error al eliminar usuario", new List<string> { ex.Message });
        }
    }

    public async Task<Result<UsuarioDto>> ObtenerPorEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("El email es requerido");
            }

            var usuario = await _unitOfWork.Repository<Usuario>().FindAsync(u => u.Email == email) ?? throw new Exception("Usuario no encontrado");
            var usuarioDto = usuario.Adapt<UsuarioDto>();
            return Result<UsuarioDto>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener usuario por email: {ex.Message}");
            return Result<UsuarioDto>.FailureResult("Error al obtener usuario", [ex.Message]);
        }
    }

    public async Task<Result<UsuarioDto>> ObtenerPorIdAsync(int id)
    {
        var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(id);
        return usuario == null
            ? Result<UsuarioDto>.FailureResult("Usuario no encontrado", ["No se encontró un usuario con el ID proporcionado"])
            : Result<UsuarioDto>.SuccessResult(usuario.Adapt<UsuarioDto>());
    }

    public async Task<Result<IEnumerable<UsuarioDto>>> ObtenerTodosAsync(int PageNumber, int pageSize)
    {
        var usuarios = await _unitOfWork.Repository<Usuario>().GetAllAsync();
        return Result<IEnumerable<UsuarioDto>>.SuccessResult(usuarios.Adapt<List<UsuarioDto>>());
    }

    public async Task<Result<IEnumerable<UsuarioDto>>> ObtenerPorRolAsync(int rol)
    {
        var usuarios = await _unitOfWork.Repository<Usuario>().FindAsync(u => (int)u.Rol == rol);
        return Result<IEnumerable<UsuarioDto>>.SuccessResult(usuarios.Adapt<List<UsuarioDto>>());
    }
}
