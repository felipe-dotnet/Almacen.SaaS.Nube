using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.DatosFiscales;
using Almacen.Saas.Application.DTOs.Usuario;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Application.Validators.DatosFiscales;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;

public class DatosFiscalesService : IDatosFiscalesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatosFiscalesService> _logger;
    public DatosFiscalesService(IUnitOfWork unitOfWork, ILogger<DatosFiscalesService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DatosFiscalesDto>> CrearAsync(CrearDatosFiscalesDto dto)
    {
        try
        {
            //1. Validar
            var validator = new CrearDatosFiscalesValidator();
            var validatorResult = validator.Validate(dto);

            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await Task.FromResult(Result<DatosFiscalesDto>.FailureResult("Errores de validación", errors));
            }

            //2. Verificar si los datos fiscales ya existen para el usuario
            var datosFiscalesExistentes = await _unitOfWork.Repository<DatosFiscales>()
                .ExistsAsync(df => df.RFC == dto.RFC);

            if (datosFiscalesExistentes)
            {
                throw new Exception("El RFC para este usuario ya existen");
            }

            //3. Crear los datos fiscales
            var datosFiscales = dto.Adapt<DatosFiscales>();

            await _unitOfWork.Repository<DatosFiscales>().AddAsync(datosFiscales);
            await _unitOfWork.SaveChangesAsync();

            var datosFiscalesDto = datosFiscales.Adapt<DatosFiscalesDto>();
            return Result<DatosFiscalesDto>.SuccessResult(datosFiscalesDto, "Datos fiscales creados exitosamente");

        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener el usuario: {Message}", ex.Message);
            return Result<DatosFiscalesDto>.FailureResult("Error al obtener usuario", [ex.Message]);
        }
    }

    public async Task<Result<DatosFiscalesDto>> ActualizarAsync(ActualizarDatosFiscalesDto dto)
    {
        try
        {
            //1. Validar
            var validator = new ActualizarDatosFiscalesValidator();
            var validatorResult = validator.Validate(dto);
            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await Task.FromResult(Result<DatosFiscalesDto>.FailureResult("Errores de validación", errors));
            }

            //2. Verificar si existe el rfc y el usuario

            var datosFiscales = await _unitOfWork.Repository<DatosFiscales>()
                .ExistsAsync(df => df.RFC == dto.RFC && df.UsuarioId == dto.UsuarioId);

            if (!datosFiscales)
            {
                throw new Exception("Este usuario no tiene asociado el RFC.");
            }

            //3. Actualizar los datos fiscales
            var datosFiscalesActualizar = dto.Adapt<DatosFiscales>();
            await _unitOfWork.Repository<DatosFiscales>().UpdateAsync(datosFiscalesActualizar);
            await _unitOfWork.SaveChangesAsync();

            var datosFiscalesDto = datosFiscalesActualizar.Adapt<DatosFiscalesDto>();
            return Result<DatosFiscalesDto>.SuccessResult(datosFiscalesDto, "Datos fiscales actualizados exitosamente");
        }
        catch (Exception ex)
        {

            _logger.LogError("Error al obtener el usuario: {Message}", ex.Message);
            return Result<DatosFiscalesDto>.FailureResult("Error al obtener usuario", [ex.Message]);
        }
    }

    public async Task<Result> EliminarAsync(int id)
    {
        try
        {
            var existente = await _unitOfWork.Repository<DatosFiscales>().GetByIdAsync(id) ?? throw new Exception("Datos fiscales no encontrados");

            await _unitOfWork.Repository<DatosFiscales>().DeleteAsync(existente);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Datos fiscales #{Id} eliminados", id);
            return Result.SuccessResult("Datos fiscales eliminados exitosamente");

        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener los datos:{Message}", ex.Message);
            return Result.FailureResult("Error al obtener los datos", [ex.Message]);
        }
    }

    public async Task<Result<DatosFiscalesDto>> ObtenerPorClienteIdAsync(int id)
    {
        try
        {
            var datosFiscales= await _unitOfWork.Repository<DatosFiscales>()
                .FindAsync(df => df.UsuarioId == id) ?? throw new Exception("Datos fiscales no encontrados para el cliente especificado.");
            var datosFiscalesDto = datosFiscales.Adapt<DatosFiscalesDto>();
            return Result<DatosFiscalesDto>.SuccessResult(datosFiscalesDto, "Datos fiscales obtenidos exitosamente");

        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener los datos:{Message}", ex.Message);
            return Result<DatosFiscalesDto>.FailureResult("Error al obtener los datos", [ex.Message]);
        }
    }    
}
