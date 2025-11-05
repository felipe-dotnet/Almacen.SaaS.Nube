using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.DatosFiscales;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IDatosFiscalesService
{
    Task<Result<DatosFiscalesDto>> ObtenerPorClienteIdAsync(int id);
    Task<Result<DatosFiscalesDto>> CrearAsync(CrearDatosFiscalesDto dto);
    Task<Result<DatosFiscalesDto>> ActualizarAsync(ActualizarDatosFiscalesDto dto);
    Task<Result> EliminarAsync(int id);
}
